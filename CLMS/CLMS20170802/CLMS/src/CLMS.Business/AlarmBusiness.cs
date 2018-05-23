using CLMS.DataProvider;
using CLMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CLMS.Business
{
    public class AlarmBusiness
    {
        private AlarmDataProvider _provider = new AlarmDataProvider();
        private OrganBusiness _organBusiness = new OrganBusiness();
        /// <summary>
        /// Alarm搜索返回PageableData
        /// </summary>
        /// <param name="linename"></param>
        /// <param name="lineIP"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <param name="organizationIdselect">搜索条件中的OrganId</param>
        /// <param name="organId">用户的OrganId</param>
        /// <returns></returns>
        public PageableData<AlarmModel> GetPage(string linename, string organName, string lineIP, int OrganId, int limit, int offset)
        {
            string condition = "";
            var searchObj = new object();
            int[] organs = _organBusiness.GetChildren(OrganId).Select(m => m.Id).ToArray();

            condition = new ConditionHelper().And("LineName", ref linename, CompareType.Like)
                    .And("OrganId", organs, CompareType.In)
                    .And("OrganName", ref organName, CompareType.Like)
                    .And("IP", lineIP, CompareType.Equal)
                    .ToString();
            searchObj = new { LineName = linename, IP = lineIP, OrganName = organName, OrganId = organs };

            return _provider.GetPage(condition, searchObj, " order by Confirm asc", offset, limit);
        }

        public AlarmModel GetItem(int id)
        {
            string condition = "AND Id = @Id";
            return _provider.GetItem(condition, new { Id = id });
        }

        //public IEnumerable<AlarmLogModel> GetLogList(int alarmId)
        //{
        //    string condition = " and alarmId = @alarmId";
        //    var searchObj = new { alarmId = alarmId };
        //    return _provider.GetLog(condition, searchObj);
        //}


        public IEnumerable<AlarmModel> GetList(int organId)
        {
            int[] organs = _organBusiness.GetChildren(organId).Select(m => m.Id).ToArray();

            string condition = new ConditionHelper().And("OrganId", organs, CompareType.In)
                .And("Confirm", 0, CompareType.Equal)
                           .ToString();
            var searchObj = new { OrganId = organs, Confirm = 0 };
            return _provider.GetList(condition, searchObj, "  order by LastTime desc  LIMIT 0,100");
        }

        public IEnumerable<AlarmModel> GetListByLine(int lineId)
        {
            string condition = new ConditionHelper().And("LineId", lineId, CompareType.Equal)
                .And("Confirm", 0, CompareType.Equal)
                           .ToString();
            var searchObj = new { LineId = lineId, Confirm = 0 };
            return _provider.GetList(condition, searchObj, "  order by LastTime desc");
        }

        public IEnumerable<AlarmModel> GetView()
        {
            return _provider.GetList("", "", "");
        }

        public IEnumerable<AlarmModel> GetTop10(int organId)
        {
            IEnumerable<int> organIds = _organBusiness.GetChildren(organId).Select(n => n.Id);
            string condition = new ConditionHelper().And("OrganId", organIds, CompareType.In)
                .And("Confirm", 0, CompareType.Equal)
                           .ToString();
            var searchObj = new { OrganId = organIds, Confirm = 0 };
            return _provider.GetList(condition, searchObj, "  order by LastTime desc LIMIT 0,10");
        }

        public IEnumerable<AlarmModel> GetListContainOrganChildren(int organId)
        {
            IEnumerable<int> organIds = _organBusiness.GetChildren(organId).Select(n => n.Id);
            string condition = new ConditionHelper().And("OrganId", organIds, CompareType.In).ToString();
            object searchModel = new { OrganId = organIds };
            return _provider.GetList(condition, searchModel, " ");
        }

        public void SolveAlarm(string alarmId)
        {
            string condition = "AND Id = @Id";
            var item = _provider.GetItem(condition, new { Id = alarmId });
            if (item == null)
            {
                throw new Exception("所要处理的告警不存在，请重试");
            }
            item.Confirm = true;
            item.State = AlarmStateType.CloseAlarm;
            //item.AlarmCount = 0;
            _provider.Update(condition, item);

            new AlarmMonitorProvider().UpdateAlarmState(item.Id, AlarmStateType.CloseAlarm);
        }


        public void SolveAlarmByLine(int lineId)
        {
            string condition = "AND LineId = @LineId";
            _provider.Update(condition, new AlarmModel { LineId = lineId, Confirm = true, State = AlarmStateType.CloseAlarm });
        }

        /// <summary>
        /// 将总天数划分时间段返回时间节点报警总数的集合
        /// </summary>
        /// <param name="startTime">起始时间</param>
        /// <param name="endTime">终止时间</param>
        /// <param name="groupDays">时间组节点天数</param>
        /// <param name="totalDays">总天数</param>
        /// <returns></returns>
        public Dictionary<DateTime, int> GetLogDataForStatistics(DateTime? startTime, DateTime? endTime, int[] alarmIds, int groupDays, int totalDays)
        {
            #region condition
            string conditionIPStr = "";
            if (startTime != null)
            {
                conditionIPStr += " AND LastTime >= @startTime";
            }
            if (endTime != null)
            {
                conditionIPStr += " AND LastTime <= @endTime";
            }
            if (conditionIPStr.Length > 0)
            {
                conditionIPStr = conditionIPStr.Substring(4);
            }

            string condition = new ConditionHelper().And(conditionIPStr)
                .And("AlarmId", alarmIds, CompareType.In).ToString() + " and state > 0 order by LastTime asc";
            var searchObj = new { startTime = startTime, endTime = endTime, AlarmId = alarmIds };
            #endregion

            List<AlarmModel> list = _provider.GetList(condition, searchObj, "").ToList();
            Dictionary<DateTime, int> dicDayCount = new Dictionary<DateTime, int>();
            Dictionary<DateTime, int> result = new Dictionary<DateTime, int>();

            if (list.Count > 0)
            {
                int tempCount = 0;
                DateTime tempTime = list[0].LastTime.Date;
                dicDayCount.Add(tempTime, tempCount);

                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].LastTime.Date == tempTime)
                    {
                        dicDayCount[tempTime] += list[i].AlarmCount;
                    }
                    else
                    {
                        tempTime = list[i].LastTime.Date;
                        tempCount = list[i].AlarmCount;
                        dicDayCount.Add(tempTime, tempCount);
                    }
                }

                //开始生成节点        
                for (int i = groupDays; i < totalDays + groupDays; i++)
                {
                    if (i % groupDays == 0)
                    {
                        tempTime = startTime.Value.AddDays(i).Date;
                        tempCount = !dicDayCount.ContainsKey(tempTime) ? 0 : dicDayCount[tempTime];
                        result.Add(tempTime, tempCount);
                    }
                    else
                    {
                        result[tempTime] += !dicDayCount.ContainsKey(startTime.Value.AddDays(i).Date) ? 0 : dicDayCount[startTime.Value.AddDays(i).Date];
                    }
                }

            }
            return result;
        }
    }
}

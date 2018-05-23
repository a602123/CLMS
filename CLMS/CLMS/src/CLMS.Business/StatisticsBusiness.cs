using CLMS.DataProvider;
using CLMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CLMS.Business
{
    public class StatisticsBusiness
    {
        /// <summary>
        /// 返回综合统计页面的数据（所有）
        /// </summary>
        /// <returns></returns>
        public object GetStatisticsData(int? organId)
        {
            //List<SwitchModel> switchList = new SwitchBusiness().GetList();
            //List<StatisticsTempModel> OccupyForSwitchs = new List<StatisticsTempModel>();
            //List<StatisticsTempModel> OnlineForSwitchs = new List<StatisticsTempModel>();
            //foreach (var item in switchList)
            //{
            //    OccupyForSwitchs.Add(new StatisticsTempModel() { Name = item.Name, Data = GetOccupyData(item.Id, "", "", "") });
            //    OnlineForSwitchs.Add(new StatisticsTempModel() { Name = item.Name, Data = GetOnlineData(item.Id, "", "", "") });
            //}

            object result = new
            {
                AlartMonthData = GetAlarmCountData(30, 3, organId),
                AlartTwoMonthData = GetAlarmCountData(60, 6, organId),
                AlartSixMonthData = GetAlarmCountData(180, 18, organId),
                //NormalSumData = GetNormalData(organId),
                //OraganLinesData = GetOrganlData(organId),
                LineStateData = GetLineStateData(organId),
                OrganStateDate = GetOrganStateData(organId),
                AlarmSumData = GetAlarmData(organId)
            };
            return result;
        }
        public ChartDataModel GetLineStateData(int? organId)
        {
            var list = new LineBusiness().GetAllItems();
            ChartDataModel result = new ChartDataModel();//{ Titles = new List<string>(), Datas = new List<int>() };           
            if (organId == null)//SysAdmin
            {
                var alarmcount = list.FindAll(n => !n.ConnectState).Count;
                var normalcount = list.Count - alarmcount;
                result.Titles = new List<string> { "报警线路", "正常线路" };
                result.Datas = new List<int> { alarmcount, normalcount };
            }
            else
            {
                list = list.FindAll(n => n.OrganizationId == organId.Value);
                var alarmcount = list.FindAll(n => !n.ConnectState).Count;
                var normalcount = list.Count - alarmcount;
                result.Titles = new List<string> { "报警线路", "正常线路" };
                result.Datas = new List<int> { alarmcount, normalcount };
            }
            return result;
        }

        public ChartDataModel GetOrganStateData(int? organId)
        {
            ChartDataModel result = new ChartDataModel();//{ Titles = new List<string>(), Datas = new List<int>() };           
            if (organId == null)//SysAdmin
            {
                var item = new OrganDataProvider().GetPosition("", "");                    
                var alarmcount =  item.Where(n => n.State == 0).Count();
                var normalcount = item.Where(n => n.State == 1).Count();
                result.Titles = new List<string> { "异常网点", "正常网点" };
                result.Datas = new List<int> { alarmcount, normalcount };
            }
            else
            {
                var list = new OrganBusiness().GetChildren(organId.Value);
                
                var alarmcount = list.FindAll(n => n.State != 1).Count;
                var normalcount = list.Count - alarmcount;
                result.Titles = new List<string> { "异常网点", "正常网点" };
                result.Datas = new List<int> { alarmcount, normalcount };
            }
            return result;
        }


        public ChartDataModel GetOrganlData(int? organId)
        {
            string haslinesql = "select o.Id,o.`Name`,COUNT(o.Id) as 'Linecount' from tb_organization as o INNER  JOIN tb_line as l on o.Id=l.OrganizationId GROUP BY o.Id";
            string nolinesql = "SELECT o.`Name`,'0' as 'LineCount' from tb_organization as o where 1=1 ";//AND o.Id NOT IN @Id
            ChartDataModel result = new ChartDataModel() { Titles = new List<string>(), Datas = new List<int>() };
            var haslinelist = new OrganBusiness().GetBySql(haslinesql, null);
            result.Titles = haslinelist.Select(n => n.Name).ToList();
            result.Datas = haslinelist.Select(n => n.LineCount).ToList();
            var haslineIdArray = haslinelist.Select(n => n.Id).ToArray();
            var nolineList = new OrganBusiness().GetBySql(haslineIdArray.Length > 0 ? $"{nolinesql} AND o.Id NOT IN @Id" : nolinesql, new { Id = haslineIdArray }).ToList();
            if (nolineList != null)
            {
                nolineList.ForEach(n =>
                {
                    result.Titles.Add(n.Name);
                    result.Datas.Add(n.LineCount);
                });
            }

            return result;
        }

        public ChartDataModel GetAlarmData(int? organId)
        {
            var list = new AlarmBusiness().GetView();
            ChartDataModel result = new ChartDataModel();//{ Titles = new List<string>(), Datas = new List<int>() };           
            if (organId == null)//SysAdmin
            {
                var alarmcount = list.Count();
                var normalcount = new LineBusiness().GetNormalLine(list.Select(n => n.LineId).ToArray()).Count();
                result.Titles = new List<string> { "报警线路", "正常线路" };
                result.Datas = new List<int> { alarmcount, normalcount };
            }
            else
            {
                //IEnumerable<AlarmModel> resultlist = list?.Where(n => n.OrganId == organId);
            }
            return result;
        }

        //public object GetIPPieData(int? switchId, string startIP, string endIP, string rangeName)
        //{
        //    object result = new
        //    {
        //        OccupySumData = GetOccupyData(switchId, startIP, endIP, rangeName, null),
        //        OnlineSumData = GetOnlineData(switchId, startIP, endIP, rangeName, null),
        //    };
        //    return result;
        //}

        /// <summary>
        /// 拼接告警折线图的数据
        /// </summary>
        /// <param name="switchId"></param>
        /// <param name="startIP"></param>
        /// <param name="endIP"></param>
        /// <param name="rangeName"></param>
        /// <returns></returns>
        //public object GetAlartLineData(int? switchId, string startIP, string endIP, string rangeName)
        //{
        //    int[] alarmsIds = GetAlarmIdInConfition(switchId, startIP, endIP, rangeName).ToArray();
        //    object result = new
        //    {
        //        AlartMonthData = GetAlarmCountData(30, 5, alarmsIds),
        //        AlartTwoMonthData = GetAlarmCountData(60, 10, alarmsIds),
        //        AlartSixMonthData = GetAlarmCountData(180, 30, alarmsIds)
        //    };
        //    return result;
        //}

        /// <summary>
        /// 拿告警数据条件里所需要的 AlarmIds
        /// </summary>
        /// <param name="switchId">交换机Id</param>
        /// <param name="startIP">起始IP</param>
        /// <param name="endIP">终止IP</param>
        /// <param name="rangeName">网段名称</param>
        /// <returns></returns>
        //private List<int> GetAlarmIdInConfition(int? switchId, string startIP, string endIP, string rangeName)
        //{
        //    List<int> alarmIds = new List<int>();
        //    List<AlarmModel> alarmList = new AlarmBusiness().GetList(switchId, rangeName, startIP, endIP).ToList();
        //    foreach (var item in alarmList)
        //    {
        //        alarmIds.Add(item.Id);
        //    }
        //    return alarmIds;
        //}


        /// <summary>
        /// 拿到告警图表要求的数据机构（折线图）
        /// </summary>
        /// <param name="Days">总天数</param>
        /// <param name="partDay">时间节点天数</param>
        /// <returns></returns>
        public ChartDataModel GetAlarmCountData(int Days, int partDay, int[] alarmIds)
        {
            ChartDataModel chartAlarmMonth = new ChartDataModel() { Titles = new List<string>(), Datas = new List<int>() };

            if (alarmIds.Length > 0)
            {
                Dictionary<DateTime, int> log = new AlarmBusiness().GetLogDataForStatistics(DateTime.Now.AddDays((-1) * Days), DateTime.Now, alarmIds, partDay, Days);
                foreach (var item in log)
                {
                    chartAlarmMonth.Titles.Add(item.Key.ToString("yyyy-MM-dd"));
                    chartAlarmMonth.Datas.Add(item.Value);
                }
            }
            else
            {
                for (int i = Days / partDay; i > 0; i--)
                {
                    chartAlarmMonth.Titles.Add(DateTime.Now.AddDays((-1) * i * partDay + partDay).ToString("yyyy-MM-dd"));
                    chartAlarmMonth.Datas.Add(0);
                }
            }
            return chartAlarmMonth;
        }

        /// <summary>
        /// 只有天数和分隔天数的时候，返回总告警
        /// </summary>
        /// <param name="Days"></param>
        /// <param name="partDay"></param>
        /// <returns></returns>
        public ChartDataModel GetAlarmCountData(int Days, int partDay, int? orgainId)
        {
            ChartDataModel chartAlarmMonth = new ChartDataModel() { Titles = new List<string>(), Datas = new List<int>() };
            Dictionary<DateTime, int> log = new Dictionary<DateTime, int>();
            if (orgainId == null)
            {
                log = new AlarmBusiness().GetLogDataForStatistics(DateTime.Now.AddDays((-1) * Days), DateTime.Now, null, partDay, Days);
            }
            else
            {
                int[] alarmIds = new AlarmBusiness().GetList(orgainId.Value).Select(n => n.Id).ToArray();
                log = new AlarmBusiness().GetLogDataForStatistics(DateTime.Now.AddDays((-1) * Days), DateTime.Now, alarmIds, partDay, Days);
            }

            foreach (var item in log)
            {
                chartAlarmMonth.Titles.Add(item.Key.ToString("yyyy-MM-dd"));
                chartAlarmMonth.Datas.Add(item.Value);
            }

            return chartAlarmMonth;
        }

        /// <summary>
        /// 返回IP地址占用 的图表数据
        /// </summary>
        /// <param name="switchId">未空则全部</param>
        /// <returns></returns>
        //public ChartDataModel GetOccupyData(int? switchId, string startIP, string endIP, string rangeName, int? orginId)
        //{
        //    ChartDataModel result = new ChartDataModel() { Titles = new List<string>(), Datas = new List<int>() };

        //    List<int> rangeIds = new List<int>();
        //    if (orginId == null)
        //    {
        //        rangeIds.AddRange(new IPRangeBusiness().GetList(switchId, rangeName).Select(n => n.Id).ToList());
        //    }
        //    else
        //    {
        //        IEnumerable<int> rangelist = new IPRangeBusiness().GetList(switchId, rangeName).Select(n => n.Id);
        //        IEnumerable<int> rangelistOrgan = new IPRangeBusiness().GetListWithChildren(orginId.Value).Select(n => n.Id);//圈定部门内范围
        //        rangeIds = rangelist.Intersect(rangelistOrgan).ToList();
        //    }



        //    IEnumerable<IPAddressModel> address = new IPStatusBusiness().GetListFromProc(rangeIds.ToArray(), startIP, endIP, "");
        //    result.Titles.Add("已使用");
        //    result.Titles.Add("未使用");
        //    result.Datas.Add(0);
        //    result.Datas.Add(0);

        //    foreach (var item in address)
        //    {
        //        if (string.IsNullOrEmpty(item.Mac))
        //        {
        //            result.Datas[1]++;
        //        }
        //        else
        //        {
        //            result.Datas[0]++;
        //        }
        //    }

        //    return result;
        //}

        /// <summary>
        /// 主页 显示 空闲的地址，获取部门下的所有空闲地址总数
        /// </summary>
        /// <param name="organId"></param>
        /// <returns></returns>
        //public int GetIPEnableCount(int? organId)
        //{
        //    int[] iprangeIds = organId == null ? new IPRangeBusiness().GetList(null, "").Select(n => n.Id).ToArray() : new IPRangeBusiness().GetListWithChildren(organId.Value).Select(n => n.Id).ToArray();
        //    IEnumerable<IPAddressModel> address = new IPStatusBusiness().GetListFromProc(iprangeIds.ToArray(), null, null, "");
        //    return address.Select(n => n.Mac == null).Count();
        //}


        /// <summary>
        /// 返回IP地址 在线 的图表数据
        /// </summary>
        /// <param name="switchId">未空则全部</param>
        /// <returns></returns>
        //public ChartDataModel GetOnlineData(int? switchId, string startIP, string endIP, string rangeName, int? orginId)
        //{
        //    ChartDataModel result = new ChartDataModel() { Titles = new List<string>(), Datas = new List<int>() };

        //    List<int> rangeIds = new List<int>();
        //    if (orginId == null)
        //    {
        //        rangeIds.AddRange(new IPRangeBusiness().GetList(switchId, rangeName).Select(n => n.Id).ToList());
        //    }
        //    else
        //    {
        //        IEnumerable<int> rangelist = new IPRangeBusiness().GetList(switchId, rangeName).Select(n => n.Id);
        //        IEnumerable<int> rangelistOrgan = new IPRangeBusiness().GetListWithChildren(orginId.Value).Select(n => n.Id);//圈定部门内范围
        //        rangeIds = rangelist.Intersect(rangelistOrgan).ToList();
        //    }

        //    IEnumerable<IPAddressModel> address = new IPStatusBusiness().GetListFromProc(rangeIds.ToArray(), startIP, endIP, "");
        //    result.Titles.Add("在线");
        //    result.Titles.Add("离线");
        //    result.Datas.Add(0);
        //    result.Datas.Add(0);

        //    foreach (var item in address)
        //    {
        //        if (item.OnLineState && string.IsNullOrEmpty(item.Mac))
        //        {
        //            result.Datas[0]++;
        //        }
        //        else
        //        {
        //            result.Datas[1]++;
        //        }
        //    }
        //    return result;
        //}
    }
}

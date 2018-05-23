using CLMS.DataProvider;
using CLMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CLMS.Business
{
    /// <summary>
    /// 用于向数据库中插入告警数据的 的回调业务类
    /// </summary>
    public class MonitorAlarmBusiness
    {
        public static MonitorAlarmBusiness _instance;

        public event Action<AlarmModel, string> SendMessage;

        public static MonitorAlarmBusiness GetInstance()
        {
            if (_instance == null)
            {
                _instance = new MonitorAlarmBusiness();
            }
            return _instance;
        }

        public class LineAlarmCache
        {
            public int Count { get; set; }
            public AlarmMonitorProvider Provider { get; set; }

            public LineAlarmCache()
            {
                Provider = new AlarmMonitorProvider();
            }
        }

        Dictionary<int, LineAlarmCache> _catch;


        private MonitorAlarmBusiness()
        {
            _catch = new Dictionary<int, LineAlarmCache>();
        }

        /// <summary>
        /// 返回结果，直接写入数据库(alarm,alarm_log,line)
        /// </summary>
        /// <param name="id">线路Id</param>
        /// <param name="pingState"></param>
        public void WriteAlarmToDB(int id, PingState pingState)
        {
            try
            {
                if (!_catch.ContainsKey(id))
                {
                    _catch.Add(id, new LineAlarmCache());
                }

                var alarm = MonitorADOProvider.GetInstance().GetLastestAlarm(id, AlarmType.LineNotConnected);
                if (!pingState.State)
                {
                    _catch[id].Count++;
                    if (alarm == null)
                    {
                        LineModel line = MonitorADOProvider.GetInstance().GetLineModel(id);
                        if (_catch[id].Count >= line.AlarmMax)
                        {
                            alarm = new AlarmModel()
                            {
                                AlarmCount = 1,
                                Confirm = false,
                                FirstTime = pingState.CollectTime,
                                IP = line.LineIP,
                                LineId = line.Id,
                                LineName = line.Name,
                                OrganId = line.OrganizationId,
                                OrganName = line.OrganizationName,
                                State = AlarmStateType.OnAlarm,
                                LastTime = pingState.CollectTime,
                                Type = AlarmType.LineNotConnected
                            };
                            //状态改为 正在告警
                            //MonitorADOProvider.GetInstance().InsertAlarm(alarm);
                            SendMessage?.Invoke(alarm, line.SMSTelphone);
                        }
                    }
                    else
                    {
                        //更新次数和最后时间
                        //MonitorADOProvider.GetInstance().UpdateAlarm(alarm.Id, alarm.AlarmCount + 1, pingState.CollectTime);
                    }
                }
                else
                {
                    LineModel line = MonitorADOProvider.GetInstance().GetLineModel(id);
                    MonitorADOProvider.GetInstance().RecoverAlarm(alarm.Id, DateTime.Now, AlarmStateType.Recover);
                    SendMessage?.Invoke(alarm, line.SMSTelphone);
                }



                //var alarm = _catch[id].Provider.GetLastestAlarm(id, AlarmType.LineNotConnected);
                //if (!pingState.State)
                //{
                //    _catch[id].Count++;

                //    //根据告警策略 更新 告警状态
                //    if (alarm == null)
                //    {
                //        LineModel line = new LineBusiness().GetItem(id);
                //        //肯定是有alarm的
                //        if (_catch[id].Count >= line.AlarmMax)
                //        {
                //            alarm = new AlarmModel()
                //            {
                //                AlarmCount = 1,
                //                Confirm = false,
                //                FirstTime = pingState.CollectTime,
                //                IP = line.LineIP,
                //                LineId = line.Id,
                //                LineName = line.Name,
                //                OrganId = line.OrganizationId,
                //                OrganName = line.OrganizationName,
                //                State = AlarmStateType.OnAlarm,
                //                LastTime = pingState.CollectTime,
                //                Type = AlarmType.LineNotConnected
                //            };
                //            //状态改为 正在告警
                //            _catch[id].Provider.InsertSync(alarm);
                //            SendMessage?.Invoke(alarm, line.SMSTelphone);
                //        }
                //    }
                //    else
                //    {
                //        //更新次数和最后时间
                //        string condition = "AND Id = @Id";
                //        string field = "AlarmCount=@AlarmCount,LastTime=@LastTime";
                //        _catch[id].Provider.UpdateSync(condition, field, new { Id = alarm.Id, AlarmCount = alarm.AlarmCount + 1, LastTime = pingState.CollectTime });
                //    }
                //}
                //else
                //{
                //    if (alarm != null)
                //    {
                //        LineModel line = new LineBusiness().GetItem(id);
                //        alarm.RecoverDate = pingState.CollectTime;
                //        alarm.State = AlarmStateType.Recover;
                //        //alarm.AlarmCount = _catch[id].Count;
                //        _catch[id].Provider.AddRecoverDate(alarm);
                //        SendMessage?.Invoke(alarm, line.SMSTelphone);
                //    }
                //}



            }
            catch (Exception ex)
            {

            }
        }
    }
}

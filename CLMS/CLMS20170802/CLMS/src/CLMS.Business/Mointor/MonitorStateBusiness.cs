using CLMS.DataProvider;
using CLMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CLMS.Business
{
    /// <summary>
    /// 用于修改线路状态 的回调业务类
    /// </summary>
    public class MonitorStateBusiness
    {
        public static MonitorStateBusiness _instance;


        public static MonitorStateBusiness GetInstance()
        {
            if (_instance == null)
            {
                _instance = new MonitorStateBusiness();
            }
            return _instance;
        }


        LineDataProvider _provider;

        private MonitorStateBusiness()
        {
            _provider = new LineDataProvider();
        }
        
        /// <summary>
        /// 修改线路状态、Alarm的恢复时间
        /// </summary>
        /// <param name="id">线路Id</param>
        /// <param name="pingState"></param>
        public void UpdateStateToDB(int id, PingState pingState)
        {
            try
            {
                //string condition = "AND Id = @Id";
                //var item = _provider.GetItem(condition, new { Id = id });
                //if (item == null)
                //{
                //    throw new Exception("所要修改的信息不存在，请重试");
                //}
                //string field = "ConnectState=@ConnectState,Log=@Log,CheckDate=@CheckDate";
                //item.ConnectState = pingState.State;
                //item.CheckDate = pingState.CollectTime;
                //item.Log = pingState.GetInfo();
                ////new LineDataProvider().Update(field, condition, item);
                //_provider.Update(field, condition, item);
                ////_provider.Update(field, condition, new { Id = id, ConnectState = pingState.State, Log = pingState.Info });




                if (MonitorADOProvider.GetInstance().ExistLineModel(id))
                {
                    //MonitorADOProvider.GetInstance().UpdateLineState(new LineModel() { Id = id, CheckDate = pingState.CollectTime, ConnectState = pingState.State });
                }
                else
                {
                    throw new Exception("所要修改的信息不存在，请重试");
                }
            }
            catch (Exception ex)
            {
                LogBusiness.GetInstance().Add(new LogModel() { Content = ex.Message + ex.StackTrace, Time = DateTime.Now, Type = LogType.AlarmManage, Username = "系统" });
            }

        }
    }
}

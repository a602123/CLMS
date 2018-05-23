using CLMS.Model;
using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLMS.DataProvider
{
    /// <summary>
    /// 只用于采集查询的 数据库帮助类 单例
    /// </summary>
    public class MonitorADOProvider
    {
        private static MonitorADOProvider _instance;
        private string _connStr = ConfigManage.GetInstance().GetSysConnStr();
        //private string _connStr = "server=192.168.1.234;user id = root; password=qwe123!@#;persistsecurityinfo=True;database=db_clms;";

        private MySqlConnection _connWrite;
        private MySqlCommand _cmdWrite;

        MySqlConnection _connRead;
        private MySqlCommand _cmdRead;

        MySqlConnection _connAlarmRead;
        private MySqlCommand _cmdAlarmRead;

        private StringBuilder _sqlBuilder = new StringBuilder();

        private object _lockWrite;
        private object _lockRead;
        private object _lockAlarmRead;

        private DateTime _last;

        private void AddSql(string sql)
        {
            lock (_lockWrite)
            {
                _sqlBuilder.Append(sql);

                if ((DateTime.Now - _last).TotalSeconds > 5)
                {
                    _last = DateTime.Now;
                    string sqls = _sqlBuilder.ToString();
                    _sqlBuilder.Clear();
                    ExecSql(sqls);
                    
                }
            }
        }

        public IEnumerable<AlarmModel> GetAllAlarm()
        {
            var sql = $"SELECT Id,AlarmCount,LineId FROM tb_alarm WHERE  Type = 1 AND State = 3;";
            using (var conn = new MySqlConnection(_connStr))
            {
                return conn.Query<AlarmModel>(sql);
            }
        }

        private void ExecSql(string sqls)
        {


            Task.Run(() =>
            {
                string a = sqls;
                try
                {
                    using (var conn = new MySqlConnection(_connStr))
                    {
                        conn.Execute(sqls,commandTimeout:30000);
                    }
                }
                catch (Exception ex)
                {
                    a = a;
                }

            });

        }

        public static MonitorADOProvider GetInstance()
        {
            if (_instance == null)
            {
                _instance = new MonitorADOProvider();
            }
            return _instance;
        }

        public MonitorADOProvider()
        {
            //MySqlConnection connWrite = new MySqlConnection(_connStr);
            //connWrite.Open();
            //_cmdWrite = new MySqlCommand();
            //_cmdWrite.Connection = connWrite;

            //_cmdWrite.CommandTimeout = 30000;


            _connRead = new MySqlConnection(_connStr);
            _connRead.Open();
            _cmdRead = new MySqlCommand();
            _cmdRead.CommandTimeout = 10000;
            _cmdRead.Connection = _connRead;

            _connAlarmRead = new MySqlConnection(_connStr);
            _connAlarmRead.Open();
            _cmdAlarmRead = new MySqlCommand();
            _cmdAlarmRead.CommandTimeout = 10000;
            _cmdAlarmRead.Connection = _connAlarmRead;

            _lockWrite = new object();
            _lockRead = new object();
            _lockAlarmRead = new object();
            _last = DateTime.Now;
        }




        //public bool ExistLineModel(int id)
        //{
        //    bool result = false;
        //    _cmd.CommandText = $"select id from tb_line where id={id}";
        //    if (_cmd.ExecuteScalar() != null)
        //    {
        //        result = true;
        //    }
        //    return result;
        //}

        public LineModel GetLineModel(int id)
        {
            LineModel result = null;

            _cmdRead.CommandText = $"select lineIp,Name,OrganizationId,OrganizationName from view_line_organization where id={id};";
            lock (_lockRead)
            {
                result = _connRead.Query<LineModel>($"select lineIp,Name,OrganizationId,OrganizationName from view_line_organization where id={id};").FirstOrDefault();
                //using (var reader = _cmdRead.ExecuteReader())
                //{
                //    if (reader.Read())
                //    {
                //        result = new LineModel()
                //        {
                //            Id = id,
                //            LineIP = reader.GetString(0),
                //            OrganizationId = reader.GetInt32(2),
                //            OrganizationName = reader.GetString(3),
                //            Name = reader.GetString(1)
                //        };
                //    }
                //    reader.Close();
                //}
            }
            return result;
        }

        public void InsertAlarm(AlarmModel alarm)
        {
            try
            {

                var sql = $"INSERT INTO tb_alarm(IP,Confirm,Type,LineName,OrganName,LineId,OrganId,State,FirstTime,LastTime,AlarmCount)VALUES('{alarm.IP}',{Convert.ToInt32(alarm.Confirm)},{Convert.ToInt32(alarm.Type)},'{alarm.LineName}','{alarm.OrganName}',{alarm.LineId},{alarm.OrganId},{Convert.ToInt32(alarm.State)},'{alarm.FirstTime.ToString("yyyy-MM-dd HH:mm:ss")}','{alarm.LastTime.ToString("yyyy-MM-dd HH:mm:ss")}',{alarm.AlarmCount});";
                AddSql(sql);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void UpdateAlarm(int id, AlarmStateType onAlarm)
        {
            try
            {
                var sql = $"update tb_alarm set State = { Convert.ToInt32(onAlarm)} where id = {id};";
                AddSql(sql);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public void UpdateAlarm(int id, DateTime lastTime)
        {
            try
            {
                var sql = $"update tb_alarm set AlarmCount = AlarmCount + 1,LastTime='{lastTime.ToString("yyyy-MM-dd HH:mm:ss")}' where id = {id};";
                AddSql(sql);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void RecoverAlarm(int id, DateTime recoverDate, AlarmStateType onAlarm)
        {
            try
            {
                var sql = $"update tb_alarm set State = { Convert.ToInt32(onAlarm)},RecoverDate='{recoverDate.ToString("yyyy-MM-dd HH:mm:ss")}' where id = {id};";
                AddSql(sql);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void UpdateLineState(LineModel model)
        {
            try
            {
                var sql = $"update tb_line set ConnectState={Convert.ToInt32(model.ConnectState)},CheckDate='{model.CheckDate}' where id={model.Id};";
                AddSql(sql);
            }
            catch (Exception ex)
            {
                throw;

            }
        }

        private string temp = "";

        /// <summary>
        /// 查询 线路 告警类型 限定下的最新告警Id和 AlarmCount
        /// </summary>
        /// <param name="lineId">线路Id</param>
        /// <param name="alarmType"></param>
        /// <returns>最新告警Id</returns>
        public AlarmModel GetLastestAlarm(int lineId, AlarmType alarmType)
        {
            AlarmModel result = null;
            _cmdAlarmRead.CommandText = $"SELECT Id,AlarmCount,LineId FROM tb_alarm WHERE LineID = '{lineId}' AND Type = { Convert.ToInt32(alarmType)} AND State = 3;";

            lock (_lockAlarmRead)
            {
                try
                {
                    result= _connAlarmRead.Query<AlarmModel>($"SELECT Id,AlarmCount,LineId FROM tb_alarm WHERE LineID = '{lineId}' AND Type = { Convert.ToInt32(alarmType)} AND State = 3;").FirstOrDefault();
                    //using (var reader = _cmdAlarmRead.ExecuteReader())
                    //{
                    //    if (reader.Read())
                    //    {
                    //        result = new AlarmModel()
                    //        {
                    //            Id = reader.GetInt32(0),
                    //            AlarmCount = reader.GetInt32(1)
                    //        };
                    //    }
                    //    reader.Close();
                    //}
                }
                catch (Exception ex)
                {
                    
                }
               
            }
            return result;
        }

    }
}

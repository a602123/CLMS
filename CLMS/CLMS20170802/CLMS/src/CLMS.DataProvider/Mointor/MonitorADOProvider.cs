using CLMS.Model;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CLMS.DataProvider
{
    /// <summary>
    /// 只用于采集查询的 数据库帮助类 单例
    /// </summary>
    public class MonitorADOProvider
    {
        private static MonitorADOProvider _instance;
        //private string _connStr = ConfigManage.GetInstance().GetSysConnStr();
        private string _connStr = "server=192.168.1.234;user id = root; password=qwe123!@#;persistsecurityinfo=True;database=db_clms;";
        private MySqlConnection _conn;
        private MySqlCommand _cmd;

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
            _conn = new MySqlConnection(_connStr);
            _conn.Open();
            _cmd = new MySqlCommand();
            _cmd.Connection = _conn;
            _cmd.CommandType = System.Data.CommandType.Text;
        }

        public bool ExistLineModel(int id)
        {
            bool result = false;
            _cmd.CommandText = $"select id from tb_line where id={id}";
            if (_cmd.ExecuteScalar() != null)
            {
                result = true;
            }
            return result;
        }

        public LineModel GetLineModel(int id)
        {
            LineModel result = null;
            //_cmdSelect.CommandText = $"select id,lineIp,Name,OrganizationId,OrganizationName from view_line_organization where id={id}";
            //_connSelect.Open();
            //_reader = _cmdSelect.ExecuteReader();
            //if (_reader.HasRows)
            //{
            //    _reader.Read();

            //    result = new LineModel();
            //    result.Id = id;
            //    result.LineIP = _reader["LineIP"].ToString();
            //    result.OrganizationId = Convert.ToInt32(_reader["OrganizationId"]);
            //    result.OrganizationName = _reader["OrganizationName"].ToString();
            //    result.Name = _reader["Name"].ToString();
            //}
            //_connSelect.Close();

            _cmd.CommandText = $"select CONCAT(lineIp,'-',Name,'-',OrganizationId,'-',OrganizationName) from view_line_organization where id={id}";
            object obj = _cmd.ExecuteScalar();
            if (obj != null)
            {
                temp = obj.ToString();
                result = new LineModel();
                result.Id = id;
                result.LineIP = temp.Split('-')[0];
                result.OrganizationId = Convert.ToInt32(temp.Split('-')[2]);
                result.OrganizationName = temp.Split('-')[3];
                result.Name = temp.Split('-')[1];
            }
            return result;
        }

        public int InsertAlarm(AlarmModel alarm)
        {
            _cmd.CommandText = $"INSERT INTO tb_alarm(IP,Confirm,Type,LineName,OrganName,LineId,OrganId,State,FirstTime,LastTime,AlarmCount)VALUES('{alarm.IP}',{Convert.ToInt32(alarm.Confirm)},{Convert.ToInt32(alarm.Type)},'{alarm.LineName}','{alarm.OrganName}',{alarm.LineId},{alarm.OrganId},{Convert.ToInt32(alarm.State)},'{alarm.FirstTime.ToString("yyyy-MM-dd HH:mm:ss")}','{alarm.LastTime.ToString("yyyy-MM-dd HH:mm:ss")}',{alarm.AlarmCount})";
            return _cmd.ExecuteNonQuery();
        }

        public int UpdateAlarm(int id, AlarmStateType onAlarm)
        {
            _cmd.CommandText = $"update tb_alarm set State = { Convert.ToInt32(onAlarm)} where id = {id}";
            return _cmd.ExecuteNonQuery();
        }

        public int UpdateAlarm(int id, int alarmCount, DateTime lastTime)
        {
            _cmd.CommandText = $"update tb_alarm set AlarmCount = { alarmCount},LastTime='{lastTime.ToString("yyyy-MM-dd HH:mm:ss")}' where id = {id}";
            return _cmd.ExecuteNonQuery();
        }

        public int RecoverAlarm(int id, DateTime recoverDate, AlarmStateType onAlarm)
        {
            _cmd.CommandText = $"update tb_alarm set State = { Convert.ToInt32(onAlarm)},RecoverDate='{recoverDate.ToString("yyyy-MM-dd HH:mm:ss")}' where id = {id}";
            return _cmd.ExecuteNonQuery();
        }

        public int UpdateLineState(LineModel model)
        {
            _cmd.CommandText = $"update tb_line set ConnectState={Convert.ToBoolean(model.ConnectState)},CheckDate='{model.CheckDate}' where id={model.Id}";
            return _cmd.ExecuteNonQuery();
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
            _cmd.CommandText = $"select CONCAT(id,'-',AlarmCount) from tb_alarm WHERE LineID = '{lineId}' AND Type = { Convert.ToInt32(alarmType)} AND Confirm = 0  AND ISNULL(RecoverDate)";
            object obj = _cmd.ExecuteScalar();
            if (obj != null)
            {
                temp = obj.ToString();
                result = new AlarmModel();
                result.Id = Convert.ToInt32(temp.Split('-')[0]);
                result.AlarmCount = Convert.ToInt32(temp.Split('-')[1]);
            }
            return result;


            //_reader = _cmdSelect.ExecuteReader();
            //if (_reader.HasRows)
            //{
            //    _reader.Read();
            //    result = new AlarmModel();
            //    result.Id = Convert.ToInt32(_reader["Id"]);
            //    result.AlarmCount = Convert.ToInt32(_reader["AlarmCount"]);
            //}
            //_connSelect.Close();
            //return result;
        }

        #region 基本方法
        /// <summary>
        /// 返回影响行数
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        private int ExecuteNonQuery(string sql)
        {
            _cmd.CommandText = sql;
            _cmd.CommandType = System.Data.CommandType.Text;
            return _cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 查询第一列
        /// </summary>
        /// <param name="sql">sql</param>
        /// <param name="commandParameters">commandParameters</param>
        /// <returns></returns>
        private object ExecuteScalar(string sql, params MySqlParameter[] commandParameters)
        {
            _cmd.CommandText = sql;
            _cmd.Parameters.AddRange(commandParameters);
            object val = _cmd.ExecuteScalar();
            _cmd.Parameters.Clear();
            return val;
        }

        private object ExecuteScalar(string sql)
        {
            _cmd.CommandText = sql;
            object val = _cmd.ExecuteScalar();
            return val;
        }


        /// <summary>
        /// List
        /// </summary>
        /// <returns></returns>
        private List<object> GetDataSet(string sql, params MySqlParameter[] commandParameters)
        {
            List<object> result = new List<object>();
            _cmd.CommandText = sql;
            _cmd.Parameters.AddRange(commandParameters);
            //_reader = _cmdUpdate.ExecuteReader();
            //while (_reader.Read())
            //{
            //    //Console.WriteLine(String.Format("{0}, {1}",reader[0], reader[1]));
            //}
            return result;
        }
        #endregion
    }
}

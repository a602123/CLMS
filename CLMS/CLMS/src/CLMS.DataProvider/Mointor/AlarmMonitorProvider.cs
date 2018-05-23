using CLMS.Model;
using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CLMS.DataProvider
{
    /// <summary>
    /// 在监视中使用的AlarmProvider
    /// </summary>
    public class AlarmMonitorProvider : BaseProvider
    {
        protected override string InsertStr
        {
            get
            {
                return INSERT;
            }
        }
        const string INSERT = "INSERT INTO tb_alarm(IP,Confirm,Note,Type,LineName,OrganName,LineId,OrganId,RecoverDate,State,FirstTime,LastTime,AlarmCount)VALUES(@IP,@Confirm,@Note,@Type,@LineName,@OrganName,@LineId,@OrganId,@RecoverDate,@State,@FirstTime,@LastTime,@AlarmCount)";

        public AlarmMonitorProvider(string tableName = "tb_alarm") : base(tableName)
        {
        }

        /// <summary>
        /// 只拿到 未确认 未恢复 的 最新 告警
        /// </summary>
        /// <param name="id">线路Id</param>
        /// <param name="alarmType"></param>
        /// <returns></returns>
        public AlarmModel GetLastestAlarm(int id, AlarmType alarmType)
        {
            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                string sql = $"select * from tb_alarm WHERE LineID = '{id}' AND Type = { Convert.ToInt32(alarmType)} AND Confirm = 0  AND ISNULL(RecoverDate)";
                var alarmQuery = conn.Query<AlarmModel>(sql);
                AlarmModel alarm = alarmQuery.FirstOrDefault();
                return alarm;
            }
        }

        public int Insert(LineModel model)
        {
            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                return conn.Execute(INSERT, model);
            }
        }

        public void InsertLineNotConnectedAlarm(LineModel model, AlarmType alarmType)
        {
            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                try
                {
                    string sql;

                    AlarmModel alarm = GetLastestAlarm(model.Id, alarmType);


                    if (model != null)
                    {
                        if (alarm == null)
                        {
                            //新写入一条Alarm

                            sql = $"INSERT INTO tb_alarm (IP,Confirm,Note,Type,LineName,OrganName,LineId,MessageSendState,OrganId,FirstTime,LastTime,AlarmCount) VALUES ('{model.LineIP}',0,'',{Convert.ToInt32(alarmType)},'{model.Name}','{model.OrganizationName}','{model.Id}',{0},{model.OrganizationId},'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}',1);";
                            conn.Query(sql);
                        }
                        else
                        {
                            //更新Alarm
                            if (alarm.AlarmCount > model.AlarmMax)
                            {
                                sql = $"update tb_alarm set State=3,LastTime='{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}',AlarmCount = {alarm.AlarmCount + 1} where id = {alarm.Id}";
                            }
                            else
                            {
                                sql = $"update tb_alarm set LastTime='{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}',AlarmCount = {alarm.AlarmCount + 1},AlarmLog='监测到线路名称为：{model.Name},IP：{model.LineIP}的线路 发生{alarmType.GetDescription()} ' where id = {alarm.Id}";
                            }

                            conn.Execute(sql);
                        }
                    }
                }
                catch (Exception ex)
                {
                    //打Log
                    throw;
                }
            }
        }

        public void UpdateAlarmState(int id, AlarmStateType onAlarm)
        {
            string sql = $"update tb_alarm set State = { Convert.ToInt32(onAlarm)} where id = {id}";

            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                conn.Execute(sql);
            }
        }

        public void AddRecoverDate(AlarmModel alarm)
        {
            string sql = $"update tb_alarm set RecoverDate = @RecoverDate,State = @State where Id = @Id ";

            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                conn.Execute(sql, alarm);
            }
        }

        //获取未发送的告警信息
        public IEnumerable<AlarmModel> GetAlarmMessage()
        {
            string sql = "select * from tb_alarm where MessageSendState = false";

            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                return conn.Query<AlarmModel>(sql);
            }
        }

        //将已发送的告警信息 修改状态位
        public void SetSended(int[] ids)
        {
            string sql = $"update tb_alarm set MessageSendState = true where id in ({string.Join(",", ids)})";

            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                conn.Execute(sql);
            }
        }
    }
}

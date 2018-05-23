using CLMS.Model;
using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CLMS.DataProvider
{
    public class MonitorProvider:BaseProvider
    {
        protected override string InsertStr
        {
            get
            {
                return "";
            }
        }

        public MonitorProvider(string tableName= "tb_monitor_log") :base(tableName)
        {

        }

        public void UpdateMonitorState(DateTime _lastTime)
        {
            string sql = $"UPDATE tb_config SET ConfigValue='{_lastTime.ToString("yyyy-MM-dd HH:mm:ss")}' WHERE ConfigName = 'LastTime'";
            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                conn.ExecuteAsync(sql);
            }
        }

        public Tuple<DateTime, int> GetParam()
        {
            string sql = "SELECT * FROM tb_config WHERE ConfigName in ('LastTime','Interval')";
            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                var temp = conn.Query<ConfigModel>(sql);
                ConfigModel lastTimeConfig = temp.Where(n => n.ConfigName == "LastTime").FirstOrDefault();

                DateTime lastTime = lastTimeConfig == null ? DateTime.MinValue : DateTime.Parse(lastTimeConfig.ConfigValue);

                ConfigModel intervalConfig = temp.Where(n => n.ConfigName == "Interval").FirstOrDefault();

                int interval = intervalConfig == null ? 6 : int.Parse(intervalConfig.ConfigValue);
                return new Tuple<DateTime, int>(lastTime, interval);

            }
        }
    }
}

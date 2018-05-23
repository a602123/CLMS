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
    public class SysConfigDataProvider : BaseProvider
    {

        const string UPDATE_FORMAT = "INSERT INTO `tb_config` SET `ConfigName`='{1}',`ConfigValue`='{0}' ON DUPLICATE KEY UPDATE `ConfigValue`='{0}';";

        public SysConfigDataProvider(string tbName = "tb_config") : base(tbName)
        {

        }

        protected override string InsertStr
        {
            get
            {
                return "";
            }
        }

        public string GetValue(string condition, object searchModel)
        {
            string sql = GetSelectSql(" ConfigValue ", condition);

            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                var item = conn.Query<string>(sql, searchModel).FirstOrDefault();
                return item;
            }
        }

        public IEnumerable<ConfigModel> GetList()
        {
            string sql = GetSelectSql(" * ", "");

            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                return conn.Query<ConfigModel>(sql);
            }
        }

        public Dictionary<string, string> GetConfig(string condition)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            string sql = GetSelectSql("*", condition);

            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                conn.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(reader.GetString("ConfigName"), reader.GetString("ConfigValue"));
                }
            }
            return result;
        }

        public void Update(string condition, ConfigModel model)
        {
            string sql = GetUpdateSql(" ConfigValue=@ConfigValue ", condition);
            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                conn.Execute(sql, model);
            }
        }

        public void Update(Dictionary<string, string> values)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var value in values)
            {
                sb.Append(string.Format(UPDATE_FORMAT, value.Value, value.Key));
            }
            string sql = sb.ToString();
            if (!string.IsNullOrEmpty(sql))
            {
                using (MySqlConnection conn = new MySqlConnection(_connStr))
                {
                    conn.Execute(sql);
                }
            }
        }
    }
}

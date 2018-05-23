using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CLMS.Model;
using MySql.Data.MySqlClient;
using Dapper;

namespace CLMS.DataProvider
{
    public class LogDataProvider : BaseProvider
    {
        public LogDataProvider(string tbName = "tb_log") : base(tbName)
        {
        }

        const string INSERT = "INSERT INTO tb_log (Content,Time,Type,Username) VALUES (@Content,@Time,@Type,@Username)";

        protected override string InsertStr
        {
            get
            {
                return "INSERT INTO tb_log (Content,Time,Type,Username) VALUES (@Content,@Time,@Type,@Username)";
            }
        }
        public void Insert(LogModel model)
        {
            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                conn.Execute(INSERT, model);
            }
        }

        public PageableData<LogModel> GetPage(string condition, object searchObj, string order, int begin, int end)
        {
            string sql = GetPageSql(" * ", order, condition, begin, end);
            PageableData<LogModel> result = null;
            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                var reader = conn.QueryMultiple(sql, searchObj);
                result = new PageableData<LogModel>()
                {
                    total = reader.Read<int>().FirstOrDefault(),
                    rows = reader.Read<LogModel>()
                };
            }
            return result;
        }

        public IEnumerable<LogModel> GetList(string condition, object searchObj, string order)
        {
            string sql = GetSelectSql("*", condition, order);
            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                var result = conn.Query<LogModel>(sql, searchObj);

                return result;
            }
        }
    }
}

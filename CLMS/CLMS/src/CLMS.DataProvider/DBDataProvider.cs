using Dapper;
using CLMS.Model;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CLMS.DataProvider
{
    public class DBDataProvider : BaseProvider
    {
        private static DBDataProvider _instance;
        //private static string _connStr;//系统数据库的连接字符串
        const string INSERT = "INSERT INTO tb_dataconfig(DBId,DBPassword,DBSource,DBHost,Name,Note)VALUES(@DBId,@DBPassword,@DBSource,@DBHost,@Name,@Note)";

        protected override string InsertStr
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public static DBDataProvider GetInstance()
        {
            if (_instance == null)
            {
                _instance = new DBDataProvider("tb_dataconfig");
                //_connStr = ConfigManage.GetInstance().GetSysConnStr();

            }
            return _instance;
        }

        /// <summary>
        /// 测试要设置为默认数据库 是可联通的
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool TestConnectState(int id)
        {
            bool result = false;
            try
            {
                string sql = GetSelectSqlByName("tb_dataconfig", " * ", $" and id ={id}");
                using (MySqlConnection conn = new MySqlConnection(_connStr))
                {
                    DBModel dbModel = conn.Query<DBModel>(sql).FirstOrDefault();
                    string connstrTemp = string.Format("server={0};user id={1};password={2};persistsecurityinfo=True;database={3};", dbModel.DBHost, dbModel.DBId, dbModel.DBPassword, dbModel.DBSource);
                    using (MySqlConnection connTemp = new MySqlConnection(connstrTemp))
                    {
                        connTemp.Open();
                        result = true;
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                return result;
            }
        }

        public void Del(string condition, int[] ids)
        {
            string sql = GetDeleteSql(condition);
            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                conn.Execute(sql, new { Id = ids });
            }
        }

        public void SetDefault(int id)
        {
            string sql = "update tb_config set configValue=@ConfigValue where ConfigName='DefaultData'";
            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                conn.Execute(sql, new { ConfigValue = id });
            }
        }

        public DBModel GetDefault()
        {
            //string sql = "select data.* from tb_config config left join tb_dataconfig data on config.configvalue = data.id where ConfigName='DefaultData'";
            //using (MySqlConnection conn = new MySqlConnection(_connStr))
            //{
            //    DBModel result = conn.Query<DBModel>(sql).FirstOrDefault();
            //    return result;
            //}
            return new DBModel() {
                DBHost="192.168.1.234",
                DBId="sa",
                DBPassword= "qwe123!@#",
                DBSource ="db_clms"
            };
        }

        public string GetDefaultDBConnStr()
        {
            DBModel dbModel = GetDefault();
            return string.Format("server={0};user id={1};password={2};persistsecurityinfo=True;database={3};", dbModel.DBHost, dbModel.DBId, dbModel.DBPassword, dbModel.DBSource);
        }

        private DBDataProvider(string tbName) : base(tbName)
        {
        }

        public PageableData<DBModel> GetPage(string condition, object searchObj, string order, int begin, int end)
        {
            string sql = GetPageSqlFromView("tb_dataconfig", " *", order, condition, begin, end);
            PageableData<DBModel> result = null;
            using (MySqlConnection conn = new MySqlConnection(ConfigManage.GetInstance().GetSysConnStr()))
            {
                var reader = conn.QueryMultiple(sql, searchObj);
                result = new PageableData<DBModel>()
                {
                    total = reader.Read<int>().FirstOrDefault(),
                    rows = reader.Read<DBModel>()
                };
            }
            return result;
        }

        public void Insert(DBModel model)
        {
            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                conn.Execute(INSERT, model);
            }
        }
    }
}

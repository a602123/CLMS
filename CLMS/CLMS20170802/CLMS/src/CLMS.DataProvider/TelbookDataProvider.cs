using CLMS.Model;
using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CLMS.DataProvider
{
    public class TelbookDataProvider:BaseProvider
    {
        private static TelbookDataProvider _instance;

        protected override string InsertStr
        {
            get
            {
                return "INSERT INTO tb_telbook(Name,Telephone) VALUES(@Name,@Telephone);";
            }
        }

        public static TelbookDataProvider GetInstance()
        {
            if (_instance==null)
            {
                _instance = new TelbookDataProvider("tb_telbook");
            }
            return _instance;
        }

        public TelbookDataProvider(string tbName):base(tbName)
        {
                
        }

        public PageableData<TelbookModel> GetPage(string condition, object searchObj, string order, int begin, int end)
        {
            //string sql = GetPageSqlFromView("view_user", "*", order, condition, begin, end);
            string sql = base.GetPageSql("*", order, condition, begin, end);
            PageableData<TelbookModel> result = null;
            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                var reader = conn.QueryMultiple(sql, searchObj);
                result = new PageableData<TelbookModel>()
                {
                    total = reader.Read<int>().FirstOrDefault(),
                    rows = reader.Read<TelbookModel>()
                };
            }
            return result;
        }

        public void Insert(TelbookModel model)
        {
            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                conn.Execute(InsertStr, model);
            }
        }

        public TelbookModel GetItem(string condition, object searchModel)
        {
            string sql = GetSelectSqlByName("tb_telbook", "*", condition);
            //string sql = GetSelectSqlByName("tb_user", "*", condition);
            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                var item = conn.Query<TelbookModel>(sql, searchModel).FirstOrDefault();
                return item;
            }
        }

        public IEnumerable<TelbookModel> GetList(string condition)
        {
            string sql = GetSelectSqlByName("tb_telbook", "*", condition);
            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                return conn.Query<TelbookModel>(sql);
            }
        }

        public IEnumerable<TelbookModel> GetList(string condition,object searchobj)
        {
            string sql = GetSelectSqlByName("tb_telbook", "*", condition);
            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                return conn.Query<TelbookModel>(sql,searchobj);
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

        public void Update(string condition, TelbookModel model)
        {
            string sql = GetUpdateSql("Name=@Name,Telephone=@Telephone", condition);
            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                conn.Execute(sql, model);
            }
        }


    }
}

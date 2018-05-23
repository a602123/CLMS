using CLMS.Model;
using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace CLMS.DataProvider
{
    public class LineDataProvider:BaseProvider
    {
        protected override string InsertStr
        {
            get
            {
                return "INSERT INTO tb_line(Name,Description,LineIP,OrganizationId,LineType,ServiceProvider,Pingsize,Pingtimes,Timeout,PingInterval)VALUES(@Name,@Description,@LineIP,@OrganizationId,@LineType,@ServiceProvider,@Pingsize,@Pingtimes,@Timeout,@PingInterval)";
            }
        }


        public LineDataProvider(string tbName = "tb_line") :base(tbName)
        {                
        }

        public PageableData<LineModel> GetPage(string condition, object searchObj, string order, int begin, int end)
        {
            string sql = GetPageSqlFromView("view_line_organization", "*", order, condition, begin, end);
            //string sql = base.GetPageSql("*", order, condition, begin, end);
            PageableData<LineModel> result = null;
            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                var reader = conn.QueryMultiple(sql, searchObj);
                result = new PageableData<LineModel>()
                {
                    total = reader.Read<int>().FirstOrDefault(),
                    rows = reader.Read<LineModel>()
                };
            }
            return result;
        }

        public void Insert(LineModel model)
        {
            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                conn.Execute(InsertStr, model);
            }
        }

        public void Insert(List<LineModel> list)
        {
            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                conn.Execute(InsertStr, list);
            }
        }

        public int Insert(string insertSql,LineModel model)
        {
            string sql = insertSql + ";SELECT @@Identity";
            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                //conn.Execute(insertSql, model);
                int id = conn.Query<int>(sql, model).FirstOrDefault();
                return id;
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

        public LineModel GetItem(string condition, object searchModel)
        {
            string sql = GetSelectSqlByName("view_line_organization", "*", condition);            
            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                var line = conn.Query<LineModel>(sql, searchModel).FirstOrDefault();
                return line;
            }
        }

        public IEnumerable<LineModel> GetItemByOrganId(string condition, object searchModel)
        {
            string sql = GetSelectSqlByName("view_line_organization", "*", condition);
            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                var line = conn.Query<LineModel>(sql, searchModel);
                return line;
            }
        }


        public IEnumerable<LineModel> GetList(string condition, object searchModel)
        {
            string sql = GetSelectSql("*", condition);
            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                var lines = conn.Query<LineModel>(sql, searchModel);
                return lines;
            }
        }


        public List<LineModel> GetAllItems(string condition, object searchModel, string order)
        {
            //string sql = GetSelectSql("*", condition, order);
            string sql = GetSelectSqlByName("view_line_organization", "*", condition, order);
            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                List<LineModel> items = conn.Query<LineModel>(sql, searchModel).ToList();
                return items;
            }
        }

        public void Update(string condition, LineModel model)
        {
            string sql = GetUpdateSql("Description=@Description,LineIP=@LineIP,OrganizationId=@OrganizationId,LineType=@LineType,ServiceProvider = @ServiceProvider,Pingsize=@Pingsize,Pingtimes=@Pingtimes,Timeout=@Timeout,tb_line.`PingInterval`=@PingInterval,AlarmMax = @AlarmMax", condition);
            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                conn.Execute(sql, model);
            }
        }

        public void Update(string field,string condition, LineModel model)
        {
            string sql = GetUpdateSql(field, condition);
            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                conn.ExecuteAsync(sql, model);
            }
        }

        public void UpdateState(int id, bool state)
        {
            string sql = GetUpdateSql($"ConnectState={state}", $" AND Id={id}");
            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                conn.Execute(sql);
            }
        }
    }
}

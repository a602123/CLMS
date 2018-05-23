using CLMS.Model;
using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CLMS.DataProvider
{
    public class OrganDataProvider : BaseProvider
    {
        const string INSERT = "INSERT INTO tb_organization(Name,Description,ParentId,SMSTelephone)VALUES(@Name,@Description,@ParentId,@SMSTelephone)";

        protected override string InsertStr
        {
            get
            {
                return "INSERT INTO tb_organization(Name,Description,ParentId)VALUES(@Name,@Description,@ParentId)";
            }
        }

        public OrganDataProvider(string tbName = "tb_organization") : base(tbName)
        {

        }

        public object GetPage(string condition, object searchObj, string order, int offset, int limit)
        {
            string sql = GetPageSqlFromView("tb_organization", "*", order, condition, offset, limit);
            PageableData<OrganModel> result = null;
            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                var reader = conn.QueryMultiple(sql, searchObj);
                result = new PageableData<OrganModel>()
                {
                    total = reader.Read<int>().FirstOrDefault(),
                    rows = reader.Read<OrganModel>()
                };
            }
            return result;
        }

        public List<OrganModel> GetPosition(string condition, object searchObj)
        {
            string sql = $"select * from view_map where 1=1 {condition}; ";

            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                List<OrganModel> items = conn.Query<OrganModel>(sql, searchObj).ToList();
                return items;
            }
        }

        public void Update(string condition, OrganModel model)
        {
            string sql = GetUpdateSql("Name=@Name,Description=@Description,SMSTelephone=@SMSTelephone,X=@X,Y=@Y", condition);
            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                conn.Open();
                conn.Execute(sql, model);
            }
        }

        public void ChangeState(string condition, OrganModel item)
        {
            string sql = GetUpdateSql(
           "State = @State"
               , condition);
            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                conn.Execute(sql, new { Id = item.Id, State = item.State });
            }
        }

        public OrganModel GetItem(string condition, object searchModel)
        {
            //string sql = GetSelectSql("*", condition);
            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                var item = conn.Query<OrganModel>("select o.Id,o.Name,p_o.Name as ParentOrganizationName,o.parentId,o.Description,o.State,o.SMSTelephone  from tb_organization o left join tb_organization p_o on o.parentId = p_o.Id where o.Id= @Id", searchModel).FirstOrDefault();
                return item;
            }
        }
        public OrganModel GetItemByCondition(string condition, object searchobj)
        {
            string sql = GetSelectSql("*", condition);
            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                var item = conn.Query<OrganModel>(sql, searchobj).FirstOrDefault();
                return item;
            }
        }
        public IEnumerable<OrganModel> GetListBySql(string sql, object searchModel)
        {
            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                IEnumerable<OrganModel> items = conn.Query<OrganModel>(sql, searchModel);
                return items;
            }
        }


        public List<OrganModel> GetAllItems(string condition, object searchModel, string order)
        {
            string sql = GetSelectSql("*", condition, order);

            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                List<OrganModel> items = conn.Query<OrganModel>(sql, searchModel).ToList();
                return items;
            }
        }
        public List<OrganModel> GetAllItemsByView(string condition, object searchModel, string order)
        {
            //string sql = GetSelectSql("*", condition, order);
            string sql = GetSelectSqlByName("view_organization", "*", condition, order);

            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                List<OrganModel> items = conn.Query<OrganModel>(sql, searchModel).ToList();
                return items;
            }
        }

        public void Insert(OrganModel model)
        {
            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                conn.Open();
                conn.Execute(INSERT, model);
            }
        }

        public void Insert(List<OrganModel> list)
        {
            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                conn.Open();
                conn.Execute("INSERT INTO tb_organization(Name,Description,ParentId,SMSTelephone,X,Y)VALUES(@Name,@Description,@ParentId,@SMSTelephone,@X,@Y)", list);
            }
        }

        public void Del(string condition, int id)
        {
            string sql = GetDeleteSql(condition);
            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                conn.Execute(sql, new { Id = id });
            }
        }
    }
}

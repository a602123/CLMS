using Dapper;
using CLMS.Model;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace CLMS.DataProvider
{
    public class UserDataProvider : BaseProvider
    {
        private static UserDataProvider _instance;       
        const string INSERT = "INSERT INTO tb_user(Username,Email,Telphone,RealName,Password,OrganId)VALUES(@Username,@Email,@Telphone,@RealName,@Password,@OrganId)";

        protected override string InsertStr
        {
            get
            {
                return "INSERT INTO tb_user(Username,Email,Telphone,RealName,Password,OrganId)VALUES(@Username,@Email,@Telphone,@RealName,@Password,@OrganId)";
            }
        }

        public static UserDataProvider GetInstance()
        {
            if (_instance == null)
            {
                _instance = new UserDataProvider("tb_user");
               // connStr = DBDataProvider.GetInstance().GetDefaultDBConnStr();
                //connStr = ConnStrManage.GetInstance().GetSysConnStr();

            }
            return _instance;
        }

        private UserDataProvider(string tbName) : base(tbName)
        {
        }



        public PageableData<UserModel> GetPage(string condition, object searchObj, string order, int begin, int end)
        {
            string sql = GetPageSqlFromView("view_user", "*", order, condition, begin, end);
            //string sql = base.GetPageSql("*", order, condition, begin, end);
            PageableData<UserModel> result = null;
            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                var reader = conn.QueryMultiple(sql, searchObj);
                result = new PageableData<UserModel>()
                {
                    total = reader.Read<int>().FirstOrDefault(),
                    rows = reader.Read<UserModel>()
                };
            }
            return result;
        }

        public void Insert(UserModel model)
        {
            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                conn.Execute(INSERT, model);
            }
        }

        public UserModel GetItem(string condition, object searchModel)
        {
            string sql = GetSelectSqlByName("view_user", "*", condition);
            //string sql = GetSelectSqlByName("tb_user", "*", condition);
            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                var user = conn.Query<UserModel>(sql, searchModel).FirstOrDefault();
                return user;
            }
        }

        public void UpdatePwd(string condition, UserModel userModel)
        {
            string sql = GetUpdateSql("Password=@Password", condition);
            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                conn.Execute(sql, userModel);
            }
        }

        public IEnumerable<UserModel> GetList(string condition)
        {
            string sql = GetSelectSqlByName("view_user", "*", condition);
            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                return conn.Query<UserModel>(sql);
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

        public void Update(string condition, UserModel model)
        {
            string sql = GetUpdateSql("Username=@Username,Email=@Email,Telphone=@Telphone,RealName=@RealName,RoleId=@RoleId,OrganId = @OrganId", condition);
            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                conn.Execute(sql, model);
            }
        }
        
        public void ChangeState(string condition, int[] ids, bool state)
        {
            string sql = GetUpdateSql(
                       "State = @State"
                           , condition);
            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                conn.Execute(sql, new { Id = ids, State = state });
            }
        }

        public void ChangeState(string condition, UserModel model)
        {
            string sql = GetUpdateSql(
                       "State = @State"
                           , condition);
            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                conn.Execute(sql, new { Id = model.Id, State = model.State });
            }
        }

    }
}

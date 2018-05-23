using CLMS.Model;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace CLMS.DataProvider
{
    public class RoleDataProvider:BaseProvider
    {
        private static RoleDataProvider _instance;

        protected override string InsertStr
        {
            get
            {
                return "";
            }
        }

        public static RoleDataProvider GetInstance()
        {
            if (_instance==null)
            {
                _instance = new RoleDataProvider("tb_role");                
            }
            return _instance;
        }

        public RoleDataProvider(string tbName):base(tbName)
        {
        }

        public List<RoleModel> GetList()
        {
            string sql = "select * from tb_role";
            List<RoleModel> result = new List<RoleModel>() { };
            using (MySqlConnection conn=new MySqlConnection(_connStr))
            {
                result = conn.Query<RoleModel>(sql, null).ToList();
            }
            return result;
        }
    }
}

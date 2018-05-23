using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CLMS.DataProvider
{
    public abstract class BaseProvider
    {

        protected string _connStr;
        public BaseProvider(string tableName)
        {
            _tableName = tableName;
            _connStr = ConfigManage.GetInstance().GetSysConnStr();
        }
        //protected abstract string InsertStr { get; }
        private string _tableName;

        const string COUNTSQL = "SELECT COUNT(*) FROM {0} WHERE  1=1 {1};";

        const string PAGESQL = "SELECT {0}  FROM {2} WHERE 1=1 {3} {1} limit {4} , {5}; ";

        const string SELECT = "SELECT {0} FROM {1} WHERE 1=1 {2} {3}";

        const string UPDATE = "UPDATE {0} SET {1} WHERE 1=1 {2}";

        const string DELETE = "DELETE FROM {0} WHERE 1=1 {1}";


        protected string GetSelectSql(string fields, string condition)
        {
            return GetSelectSqlByName(_tableName, fields, condition, "");
        }

        protected string GetSelectSql(string fields, string condition, string order)
        {
            return GetSelectSqlByName(_tableName, fields, condition, order);
        }

        protected string GetSelectSqlByName(string tableName, string fields, string condition)
        {
            return string.Format(SELECT, fields, tableName, condition, "");
        }

        protected string GetSelectSqlByName(string tableName, string fields, string condition, string order)
        {
            return string.Format(SELECT, fields, tableName, condition, order);
        }

        protected string GetDeleteSql(string condition)
        {
            return GetDeleteSqlByName(_tableName, condition);
        }


        protected string GetDeleteSqlByName(string tableName, string condition)
        {
            return string.Format(DELETE, tableName, condition);
        }

        protected string GetUpdateSql(string fields, string condition)
        {
            return string.Format(UPDATE, _tableName, fields, condition);
        }

        protected string GetPageSql(string fields, string order, string condition, int begin, int end)
        {
            string countStr = string.Format(COUNTSQL, _tableName, condition);
            string rowsStr = string.Format(PAGESQL, fields, order, _tableName, condition, begin.ToString(), end.ToString());
            return string.Format("{0}{1}", countStr, rowsStr);
        }

        protected string GetPageSqlFromView(string viewName, string fields, string order, string condition, int begin, int end)
        {
            string countStr = string.Format(COUNTSQL, viewName, condition);
            string rowsStr = string.Format(PAGESQL, fields, order, viewName, condition, begin, end);
            return string.Format("{0}{1}", countStr, rowsStr);
        }


        protected string GetMallPageSql(string fields, string order, string condition, int begin, int end)
        {
            string rowsStr = string.Format(PAGESQL, fields, order, _tableName, condition, begin.ToString(), end.ToString());
            return rowsStr;
        }

        protected string GetMallPageSqlFromView(string viewName, string fields, string order, string condition, int begin, int end)
        {
            string rowsStr = string.Format(PAGESQL, fields, order, viewName, condition, begin, end);
            return rowsStr;
        }

        #region 公开的方法
        protected abstract string InsertStr { get; }

        public async Task<bool> Insert(object model)
        {
            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                return await conn.ExecuteAsync(InsertStr, model) > 0;
            }
        }

        public bool InsertSync(object model)
        {
            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                return conn.Execute(InsertStr, model) > 0;
            }
        }

        public async Task<bool> Delete(string condition)
        {
            string sql = GetDeleteSql(condition);
            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                return await conn.ExecuteAsync(sql) > 0;
            }
        }

        public async Task<bool> Update(string condition, string fields, object model)
        {
            string sql = GetUpdateSql(fields, condition);
            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                return await conn.ExecuteAsync(sql, model) > 0;
            }
        }

        public bool UpdateSync(string condition, string fields, object model)
        {
            string sql = GetUpdateSql(fields, condition);
            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                return conn.Execute(sql, model) > 0;
            }
        }

        public async Task<IEnumerable<T>> GetList<T>(string condition, object searchObj)
        {
            string sql = GetSelectSql("*", condition);
            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                return await conn.QueryAsync<T>(sql, searchObj);
            }
        }


        public async Task<IEnumerable<T>> GetList<T>(string condition, object searchObj, string viewName)
        {
            string sql = GetSelectSqlByName(viewName, "*", condition);
            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                return await conn.QueryAsync<T>(sql, searchObj);
            }
        }

        public async Task ExecSql(string sql)
        {
            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                await conn.ExecuteAsync(sql);
            }
        }
        #endregion 
    }
}

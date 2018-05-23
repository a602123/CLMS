using CLMS.Model;
using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CLMS.DataProvider
{
    /// <summary>
    /// 在前台 查询时使用的AlarmProvider
    /// </summary>
    public class AlarmDataProvider : BaseProvider
    {
        protected override string InsertStr
        {
            get
            {
                return "";
            }
        }

        public AlarmDataProvider(string tableName = "tb_alarm") : base(tableName)
        {
        }

        public IEnumerable<AlarmModel> GetList(string condition, object searchObj, string order)
        {
            string sql = GetSelectSqlByName("tb_alarm", "*", condition, order);
            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                var result = conn.Query<AlarmModel>(sql, searchObj);
                //result.ToList().ForEach(delegate (AlarmModel n)
                //{
                //n.LastTime = string.IsNullOrEmpty(n.LastTime) ? "" : n.LastTime.Replace('T', ' ');
                //n.FirstTime = string.IsNullOrEmpty(n.FirstTime) ? "" : n.FirstTime.Replace('T', ' ');
                //    n.TypeStr = ((AlarmType)n.Type).GetDescription();
                //});
                return result;
            }
        }

        public PageableData<AlarmModel> GetPage(string condition, object searchObj, string order, int begin, int end)
        {
            string sql = GetPageSqlFromView("tb_alarm", " * ", order, condition, begin, end);
            //string sql = GetSelectSqlByName("view_alarm", "*", condition, order);
            PageableData<AlarmModel> result = null;

            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                var reader = conn.QueryMultiple(sql, searchObj);
                result = new PageableData<AlarmModel>()
                {
                    total = reader.Read<int>().FirstOrDefault(),
                    rows = reader.Read<AlarmModel>()
                };
                //var result = conn.Query<AlarmModel>(sql, searchObj);
                //result.rows.ToList().ForEach(delegate (AlarmModel n)
                //{
                //    n.LastTime = string.IsNullOrEmpty(n.LastTime) ? "" : n.LastTime.Replace('T', ' ');
                //    n.FirstTime = string.IsNullOrEmpty(n.FirstTime) ? "" : n.FirstTime.Replace('T', ' ');
                //    n.TypeStr = ((AlarmType)n.Type).GetDescription();
                //});
                return result;
            }
        }

        public IEnumerable<AlarmModel> GetAlarmMessage()
        {
            string sql = "select * from tb_alarm where MessageSendState = false";

            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                return conn.Query<AlarmModel>(sql);
            }
        }

        public void SetSended(int[] ids)
        {
            string sql = $"update tb_alarm set MessageSendState = true where id in ({string.Join(",", ids)})";

            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                conn.Execute(sql);
            }
        }

        public IEnumerable<AlarmLogModel> GetLog(string condition, object searchObj)
        {
            string sql = GetSelectSqlByName("tb_alarm_log", "*", condition);
            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                var result = conn.Query<AlarmLogModel>(sql, searchObj);
                return result;
            }
        }

        public AlarmModel GetItem(string condition, object searchModel)
        {
            string sql = string.Format("select * from tb_alarm where 1=1 {0} ", condition);

            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                var item = conn.Query<AlarmModel>(sql, searchModel).FirstOrDefault();
                return item;
            }
        }

        /// <summary>
        /// 只修改告警状态，确认状态
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="item"></param>
        public void Update(string condition, AlarmModel item)
        {
            string sql = string.Format("update tb_alarm set Confirm = @Confirm,State = {1}  where 1=1 {0}", condition, Convert.ToInt32(item.State));
            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                conn.Execute(sql, item);
            }
        }

        public int GetNotSolvedAlarmCount(string condition, object searchModel)
        {
            string sql = string.Format("select count(*) from tb_alarm where 1=1 and confirm=0 {0}", condition);

            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                var item = conn.Query<int>(sql, searchModel).FirstOrDefault();
                return item;
            }
        }
    }
}

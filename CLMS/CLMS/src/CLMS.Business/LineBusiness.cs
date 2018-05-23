using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CLMS.Model;
using CLMS.DataProvider;
using ZZBFBY.CommonTools_Core.IPHelper;

namespace CLMS.Business
{
    public class LineBusiness
    {
        private LineDataProvider _provider;
        private OrganBusiness _organBusiness = new OrganBusiness();

        public LineBusiness()
        {
            _provider = new LineDataProvider();
        }

        /// <summary>
        /// 返回Line的页数据
        /// </summary>
        /// <param name="organizationName"></param>
        /// <param name="name"></param>
        /// <param name="lineIP"></param>
        /// <param name="user"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public PageableData<LineModel> GetPage(string organizationName, string name, string lineIP, int OrganId, int limit, int offset)
        {
            string condition = "";
            var searchObj = new object();
            int[] organs = _organBusiness.GetChildren(OrganId).Select(m => m.Id).ToArray();
            condition = new ConditionHelper().And("OrganizationName", ref organizationName, CompareType.Like)
                    .And("Name", ref name, CompareType.Like)
                    .And("OrganizationId", organs, CompareType.In)
                    .And("LineIP", lineIP, CompareType.Equal)
                    .ToString();
            searchObj = new { OrganizationName = organizationName, Name = name, LineIP = lineIP, OrganizationId = organs };

            return _provider.GetPage(condition, searchObj, " ", offset, limit);
        }

        public LineModel Insert(LineModel model)
        {
            var item = _provider.GetItem(" AND Name=@Name", new { Name = model.Name });
            string baseSql = "INSERT INTO tb_line(Name,Description,LineIP,OrganizationId,LineType,ServiceProvider,{0})VALUES(@Name,@Description,@LineIP,@OrganizationId,@LineType,@ServiceProvider,{1})";
            string sql = "INSERT INTO tb_line(Name,Description,LineIP,OrganizationId,LineType,ServiceProvider)VALUES(@Name,@Description,@LineIP,@OrganizationId,@LineType,@ServiceProvider)";
            string field = string.Empty;
            string parameter = string.Empty;
            if (item != null)
            {
                throw new Exception("已存在相同用户名的用户");
            }

            if (model.Pingtimes != 0)
            {
                field += "Pingtimes,";
                parameter += "@Pingtimes,";
            }
            if (model.Pingsize != 0)
            {
                field += "Pingsize,";
                parameter += "@Pingsize,";
            }
            if (model.Timeout != 0)
            {
                field += "Timeout,";
                parameter += "@Timeout,";
            }
            if (model.PingInterval != 0)
            {
                field += "PingInterval,";
                parameter += "@PingInterval,";
            }
            if (model.AlarmMax != 0)
            {
                field += "AlarmMax,";
                parameter += "@AlarmMax,";
            }

            //string.Format(baseSql, field.Substring(0, field.Length - 1), parameter.Substring(0, parameter.Length - 1));
            var insertsql = field.Length > 0 ? string.Format(baseSql, field.Substring(0, field.Length - 1), parameter.Substring(0, parameter.Length - 1)) : sql;
            int id = _provider.Insert(insertsql, model);
            return GetItem(id);
        }

        public LineModel GetItem(int id)
        {
            string condition = "AND Id = @Id";
            return _provider.GetItem(condition, new { Id = id });
        }

        public List<LineModel> GetAllItems()
        {
            var items = _provider.GetAllItems("", null, " order by Id ASC");
            return items;
        }


        public List<LineModel> GetList(int organId)
        {
            var items = _provider.GetAllItems(" and OrganizationId = @OrganizationId", new { OrganizationId = organId }, " order by Id ASC");
            return items;
        }

        public void Update(LineModel model)
        {
            string condition = "AND Id = @Id";
            var item = _provider.GetItem(condition, new { Id = model.Id });
            if (item == null)
            {
                throw new Exception("所要修改的信息不存在，请重试");
            }
            _provider.Update(condition, model);
        }

        public void Del(int[] ids)
        {
            string condition = new ConditionHelper().And("Id", new { Id = ids }, CompareType.In).ToString();
            _provider.Del(condition, ids);
        }

        public IEnumerable<LineModel> GetNormalLine(int[] ids)
        {
            string condition = string.Empty;
            condition = ids.Length > 0 ? "AND Id NOT IN @Id" : string.Empty;
            return _provider.GetList(condition, new { Id = ids });
        }
    }
}

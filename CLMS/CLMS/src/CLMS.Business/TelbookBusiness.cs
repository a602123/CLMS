using CLMS.DataProvider;
using CLMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CLMS.Business
{
    public class TelbookBusiness
    {
        private TelbookDataProvider _provider;
        public TelbookBusiness()
        {
            _provider = TelbookDataProvider.GetInstance();
        }

        public PageableData<TelbookModel> GetPage(string name, string telephone, int limit, int offset)
        {
            string condition = new ConditionHelper().And("Name", name, CompareType.Like)
                                                   .And("Telephone", telephone, CompareType.Like)
                                                   .ToString();
            var searchObj = new
            {
                Name = $"%{name}%",
                Telephone = $"%{telephone}%"
            };
            return _provider.GetPage(condition, searchObj, "order by Id ASC", offset, limit);
        }

        public void Insert(TelbookModel model)
        {
            var item = _provider.GetItem(" AND Name=@Name", new { Name = model.Name });
            if (item != null)
            {
                throw new Exception("已存在相同姓名的人员");
            }

            _provider.Insert(model);
        }

        public object GetItem(int id)
        {
            string condition = "AND Id = @Id";
            return _provider.GetItem(condition, new { Id = id });
        }

        public IEnumerable<TelbookModel> GetList()
        {
            return _provider.GetList("");
        }

        public void Update(TelbookModel model)
        {
            string condition = "AND Id = @Id";
            var item = _provider.GetItem(condition, new { Id = model.Id });
            if (item == null)
            {
                throw new Exception("所要修改的信息不存在，请重试");
            }
            //model.Password = item.Password;
            _provider.Update(condition, model);
        }

        public void Del(params int[] ids)
        {
            string condition = new ConditionHelper().And("Id", new { Id = ids }, CompareType.In).ToString();
            _provider.Del(condition, ids);
        }

        public string GetTelephone(params string[] ids)
        {
            string condition = new ConditionHelper().And("Id", new { Id = ids }, CompareType.In).ToString();
            var searchObj = new { Id = ids };            
            var list=_provider.GetList(condition, searchObj).ToList();
            string result = string.Empty;
            list.ForEach(n =>
            {
                result += $"{n.Name}-{n.Telephone},";
            });
            if (!string.IsNullOrEmpty(result))
            {
                result = result.Substring(0, result.Length - 1);
            }
            return result;
        }
    }
}

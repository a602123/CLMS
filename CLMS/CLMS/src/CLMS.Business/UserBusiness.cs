using CLMS.DataProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CLMS.Model;
using Microsoft.AspNetCore.Http;

namespace CLMS.Business
{
    public class UserBusiness
    {
        private UserDataProvider _provider;
        public UserBusiness()
        {
            _provider = UserDataProvider.GetInstance();
        }

        public void TestFunc()
        {
            //LogWriter.LogInfo("111111");
        }

        public PageableData<UserModel> GetPage(string userName, int? state, int? typeId, int limit, int offset)
        {
            string condition = new ConditionHelper().And("Username", ref userName, CompareType.Like)
                                                   .And("State", state, CompareType.Equal)
                                                   //.And("TypeId", typeId, CompareType.Equal)
                                                   .ToString();


            var searchObj = new
            {
                Username = string.Format("%{0}%", userName),
                State = state
            };
            return _provider.GetPage(condition, searchObj, " order by Id ASC", offset, limit);
        }

        public void Insert(UserModel model)
        {
            var item = _provider.GetItem(" AND Username=@Username", new { Username = model.Username });
            if (item != null)
            {
                throw new Exception("已存在相同用户名的用户");
            }

            _provider.Insert(model);
        }

        public UserModel GetItem(int id)
        {
            string condition = "AND Id = @Id";
            return _provider.GetItem(condition, new { Id = id });
        }

        public void Update(UserModel model)
        {
            string condition = "AND Id = @Id";
            var item = _provider.GetItem(condition, new { Id = model.Id });
            if (item == null)
            {
                throw new Exception("所要修改的信息不存在，请重试");
            }
            model.Password = item.Password;
            _provider.Update(condition, model);
        }

        public void UpdatePwd(string oldPwd, string newPwd, int id)
        {
            UserModel userModel = GetItem(id);
            if (userModel.Password == oldPwd)
            {
                string condition = "AND Id = @Id";
                userModel.Password = newPwd;
                _provider.UpdatePwd(condition, userModel);
            }
            else
            {
                throw new Exception("用户密码输入错误！");
            }
        }

        public void Update(UserModel model, HttpResponse Response)
        {
            string url = string.Empty;
            string condition = "AND Id = @Id";
            var item = _provider.GetItem(condition, new { Id = model.Id });
            if (item == null)
            {
                throw new Exception("所要修改的信息不存在，请重试");
            }
            else
            {
                if (model.RoleId != item.RoleId)
                {
                    CookieHelper helper = CookieHelper.GetInstance(HelperKeys.UserIdCookieKey);
                    helper.CleanCookie(Response);
                }
            }
            _provider.Update(condition, model);
        }

        public void Ban(int id)
        {
            string condition = "AND Id = @Id";
            var item = _provider.GetItem(condition, new { Id = id });
            if (item == null)
            {
                throw new Exception("所要修改的信息不存在，请重试");
            }
            item.State = item.State == 0 ? 1 : 0;
            _provider.ChangeState(condition, item);
        }


        public void Del(int[] ids)
        {
            string condition = new ConditionHelper().And("Id", new { Id = ids }, CompareType.In).ToString();
            _provider.Del(condition, ids);
        }

        public bool Login(UserModel model, HttpResponse response)
        {
            string condition = "AND username = @username";
            var item = _provider.GetItem(condition, new { username = model.Username });
            if (item == null)
            {
                throw new Exception("用户不存在，请重试");
            }
            if (model.Password == item.Password)
            {
                var helper = CookieHelper.GetInstance(HelperKeys.UserIdCookieKey);
                helper.CreateCookies(item, response);
                return true;
            }
            else
            {
                throw new Exception("密码错误，请重试");
            }
        }

        public void ResetPassword(int id)
        {
            string condition = "AND id = @id";
            var item = _provider.GetItem(condition, new { id = id });
            if (item == null)
            {
                throw new Exception("用户不存在，请重试");
            }
            try
            {
                item.Password = new ConfigBusiness().GetConfigFromDB("").ContainsKey("DefaultPassword") ? new ConfigBusiness().GetConfigFromDB("")["DefaultPassword"] : "bfby";
            }
            catch (Exception)
            {

                item.Password = "bfby";
            }

            _provider.UpdatePwd(condition, item);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CLMS.Model;
using CLMS.Business;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CLMS.ManageSite.Controllers
{
    public class UserController : BaseController
    {
        private UserBusiness _business = new UserBusiness();
        // GET: /<controller>/        
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Search(int limit, int offset, string username, int? state, int? typeId)
        {
            var temp = _business.GetPage(username, state, typeId, limit, offset);
            PageableData<object> list = new PageableData<object>()
            {
                rows = temp.rows.Select(n => new
                {
                    n.Email,
                    n.Id,
                    n.RealName,
                    n.Username,
                    n.Telphone,
                    RoleName = n.RoleId.GetDescription(),
                    n.State,
                    n.OrganName
                }),
                total = temp.total
            };

            return Json(list);
        }

        [AuthorizationFilter(Role = UserType.SysAdmin)]
        public IActionResult Add()
        {
            return View();
        }

        [AuthorizationFilter(Role = UserType.SysAdmin | UserType.OrganAdmin)]
        public IActionResult ResetPwd()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ResetPwd(string oldPwd, string newPwd)
        {
            try
            {
                _business.UpdatePwd(oldPwd, newPwd, GetUser().Id);
                LogBusiness.GetInstance().Add(new LogModel() { Content = $" 用户 {GetUser().Username} 成功修改密码", Time = DateTime.Now, Type = LogType.UserManager, Username = GetUser().Username });
                return Json(new { State = true, Message = "密码修改成功！" });
            }
            catch (Exception ex)
            {

                return Json(new { State = false, Message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Add(UserModel model)
        {
            try
            {
                _business.Insert(model);
                return Json(new JsonResultLogModel
                {
                    State = true,
                    Message = "用户添加成功！",
                    Log = $" 用户 {GetUser().Username} 添加用户 {model.Username} 成功  ",
                    LogType = LogType.UserManager,
                    Username = GetUser().Username
                });
            }
            catch (Exception ex)
            {

                return Json(new { State = false, Message = ex.Message });
            }
        }

        public IActionResult Edit(int id)
        {
            var model = _business.GetItem(id);
            return View(model);
        }

        [HttpPost]
        [AuthorizationFilter(Role = UserType.SysAdmin)]
        public JsonResult Edit(UserModel model)
        {
            try
            {
                _business.Update(model);
                return Json(new JsonResultLogModel
                {
                    State = true,
                    Message = "用户信息修改成功！",
                    Log = $" 用户 {GetUser().Username} 编辑用户 {model.Username} 成功  ",
                    LogType = LogType.UserManager,
                    Username = GetUser().Username
                });
                //}
            }
            catch (Exception ex)
            {
                return Json(new { State = false, Message = ex.Message });
            }
        }

        [HttpPost]
        [AuthorizationFilter(Role = UserType.SysAdmin)]
        public JsonResult Ban(int id)
        {
            try
            {
                _business.Ban(id);
                return Json(new JsonResultLogModel
                {
                    State = true,
                    Message = "禁用用户成功！",
                    Log = $" 用户 {GetUser().Username} 编辑用户 {id} 状态成功  ",
                    LogType = LogType.UserManager,
                    Username = GetUser().Username
                });
            }
            catch (Exception ex)
            {
                return Json(new { State = false, Message = ex.Message });
            }
        }

        [HttpPost]
        [AuthorizationFilter(Role = UserType.SysAdmin)]
        public JsonResult Del(int[] ids)
        {
            try
            {
                _business.Del(ids);
                return Json(new JsonResultLogModel
                {
                    State = true,
                    Message = "删除用户成功！",
                    Log = $" 用户 {GetUser().Username} 删除用户 Id:{string.Join(",", ids)} 成功  ",
                    LogType = LogType.UserManager,
                    Username = GetUser().Username
                });

            }
            catch (Exception ex)
            {
                return Json(new { State = false, Message = ex.Message });
            }
        }

        [HttpPost]
        [AuthorizationFilter(Role = UserType.SysAdmin)]
        public JsonResult ResetPassword(int id)
        {
            try
            {
                _business.ResetPassword(id);
                return Json(new JsonResultLogModel
                {
                    State = true,
                    Message = "重置密码成功！",
                    Log = $" 用户 {GetUser().Username} 重置 用户 Id:{id} 密码 成功  ",
                    LogType = LogType.UserManager,
                    Username = GetUser().Username
                });
            }
            catch (Exception ex)
            {
                return Json(new { State = false, Message = ex.Message });
            }
        }
    }
}

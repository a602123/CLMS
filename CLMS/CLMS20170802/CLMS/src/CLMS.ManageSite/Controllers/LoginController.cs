using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CLMS.Model;
using CLMS.Business;

namespace CLMS.ManageSite.Controllers
{
    public class LoginController : BaseController
    {
        private UserBusiness _business = new UserBusiness();

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Login(UserModel model)
        {
            try
            {
                if (_business.Login(model,Response))
                {
                    return Json(
                        new JsonResultLogModel
                        {
                            State = true,
                            Message = "登陆成功",
                            Log = $"用户“{model.Username}”于{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}登陆系统",
                            LogType = LogType.UserLogin,
                            Username = model.Username
                        }
                    );
                }
                return Json(null);

            }
            catch (Exception ex)
            {

                return Json(new { State = false, Message = ex.Message });
            }
        }
    }
}
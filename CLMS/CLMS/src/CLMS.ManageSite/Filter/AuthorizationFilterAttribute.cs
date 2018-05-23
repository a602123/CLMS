using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using CLMS.Model;
using CLMS.Business;

namespace CLMS.ManageSite
{
    public class AuthorizationFilterAttribute : Attribute, IAuthorizationFilter
    {
        public UserType Role { get; set; }

        private CookieHelper _helper = CookieHelper.GetInstance(HelperKeys.UserIdCookieKey);

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            UserModel user = _helper.GetCookie<UserModel>(context.HttpContext.Request);

            if (user == null)
            {
                context.Result = new RedirectResult("/Login/Index");
            }
            try
            {

                if (!Role.HasFlag(user.RoleId))
                {
                    if (context.HttpContext.Request.Method.ToUpper() == "POST")
                    {
                        context.Result = new JsonResult(new JsonResultBaseModel { State = false, Message = "没有此项操作权限" });
                    }
                    else
                    {
                        context.Result = new RedirectResult("/Home/NoRole");
                    }
                }

            }
            catch (Exception ex)
            {
                if (context.HttpContext.Request.Method.ToUpper() == "POST")
                {
                    context.Result = new JsonResult(new JsonResultBaseModel { State = false, Message = "没有此项操作权限" });
                }
                else
                {
                    context.Result = new RedirectResult("/Login/Index");
                }
            }
        }
    }
}


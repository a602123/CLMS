using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CLMS.Business;
using CLMS.Model;

namespace CLMS.ManageSite.Controllers
{
    [ValidFilter]
    public class HomeController : Controller
    {
        [AuthorizationFilter(Role = UserType.SysAdmin | UserType.OrganAdmin)]
        public IActionResult Index()
        {
            var user = CookieHelper.GetInstance(HelperKeys.UserIdCookieKey).GetCookie<UserModel>(Request);
            ViewBag.AlarmCount=new AlarmBusiness().GetList(user.OrganId).Count();
            ViewBag.OrganCount = new OrganBusiness().GetChildren(user.OrganId).Count();
            var list=new LineBusiness().GetAllItems();
            int lineCount = user.RoleId == UserType.SysAdmin ?list.Count():list.Where(n=>n.OrganizationId==user.OrganId).Count();
            ViewBag.LineCount = lineCount;

            //if (user.RoleId == UserType.OrganAdmin)
            //{
            //    return Redirect($"/Login/Index/");
            //}            
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            Ping ping = new Ping();
            return View();
        }

        public IActionResult NoRole()
        {
            return View();
        }

        public JsonResult LogOut()
        {
            try
            {
                CookieHelper helper = CookieHelper.GetInstance(HelperKeys.UserIdCookieKey);
                helper.CleanCookie(Response);
                return Json(new { State = true });
            }
            catch (Exception ex)
            {
                return Json(new { State = false, Message = ex.Message });
            }
        }


        public IActionResult Monitor()
        {
            return View();
        }
    }
}

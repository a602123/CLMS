using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CLMS.Business;
using CLMS.Model;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CLMS.ManageSite.Controllers
{
    [ValidFilter]
    public class StatisticsController : BaseController
    {
        private StatisticsBusiness _business = new StatisticsBusiness();
        // GET: /<controller>/
        [AuthorizationFilter(Role =UserType.SysAdmin)]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 综合统计页的数据
        /// </summary>
        /// <returns></returns>
        public JsonResult GetData()
        {
            try
            {
                var user = CookieHelper.GetInstance(HelperKeys.UserIdCookieKey).GetCookie<UserModel>(Request);
                if (user.RoleId == UserType.SysAdmin)
                {
                    return Json(new { data = _business.GetStatisticsData(null) });
                }
                else
                {
                    return Json(new { data = _business.GetStatisticsData(user.OrganId) });
                }
            }
            catch (Exception ex)
            {
                return Json(new { State = false, Message = ex.Message });
            }
        }

        [AuthorizationFilter(Role = UserType.SysAdmin)]
        public IActionResult DetailChart()
        {
            return View();
        }
    }
}

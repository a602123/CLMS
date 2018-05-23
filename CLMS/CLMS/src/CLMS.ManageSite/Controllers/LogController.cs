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
    public class LogController : BaseController
    {
        private LogBusiness _business = new LogBusiness();
        // GET: /<controller>/
        [AuthorizationFilter(Role =UserType.SysAdmin)]
        public IActionResult Index()
        {            
            return View();
        }

        [HttpPost]
        public JsonResult Search(int limit, int offset,DateTime? startTime, DateTime? endTime,int? LogType)
        {
            PageableData<LogModel> temp = _business.GetPage(LogType, startTime, endTime, limit, offset);
            PageableData<object> list = new PageableData<object>()
            {
                total = temp.total,
                rows = temp.rows.Select(n => new
                {
                    n.Id,
                    n.Content,
                    Time = n.Time.ToString("yyyy-MM-dd HH:mm:ss"),
                    n.Username,
                    Type = n.Type.GetDescription()
                })
            };
            return Json(list);
        }

        [HttpPost]
        public JsonResult SearchTop10()
        {
            var list = _business.GetTop10().Select(n => new
            {
                n.Id,
                n.Content,
                Time = n.Time.ToString("yyyy-MM-dd HH:mm:ss"),
                n.Username,
                Type = n.Type.GetDescription()
            });
            return Json(list);
        }
    }
}

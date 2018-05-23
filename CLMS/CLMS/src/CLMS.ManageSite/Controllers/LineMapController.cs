using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CLMS.Business;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CLMS.ManageSite.Controllers
{
    [ValidFilter]
    public class LineMapController : BaseController
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        public JsonResult GetMapUrl()
        {
            try
            {
                return Json(new { State = true, Map = new ConfigBusiness().GetConfigList()["LineMap"] });
            }
            catch (Exception ex)
            {
                return Json(new { State = false, Message = ex.Message });
            }
        }
    }
}

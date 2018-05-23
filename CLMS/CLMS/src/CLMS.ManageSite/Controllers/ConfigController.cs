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
    public class ConfigController : BaseController
    {
        private ConfigBusiness _business = new ConfigBusiness();
        // GET: /<controller>/
        public IActionResult Index()
        {
            ViewBag.Configs = _business.GetConfigList();

            return View();
        }

        [HttpPost]
        public JsonResult SysConfigEdit(string sysName, string rootName, string defaultPassword)
        {
            try
            {
                _business.SysConfigEdit(sysName, rootName, defaultPassword);
                return Json(new JsonResultLogModel
                {
                    State = true,
                    Message = "修改成功！",
                    Log = $" 用户 {SiteUser.Username} 修改系统配置: 系统名称 为 “{sysName}”；分行名称 为“{rootName}”；默认重置密码 为 “{defaultPassword}”",
                    LogType = LogType.ConfigManage,
                    Username = SiteUser.Username
                });
            }
            catch (Exception ex)
            {
                return Json(new { State = false, Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult PingConfigEdit(string interval,string pingtimes, string pingsize,string pingttl)
        {
            try
            {
                _business.PingConfigEdit(interval,pingtimes,pingsize,pingttl);
                return Json(new JsonResultLogModel
                {
                    State = true,
                    Message = "修改成功！",
                    Log = $" 用户 {SiteUser.Username} 修改Ping配置: 轮询间隔 为 “{interval}”小时；Ping次数 为“{pingtimes}”次；Ping包的大小 为 “{pingsize}”字节；Ping包的转发次数 为“{pingttl}”",
                    LogType = LogType.ConfigManage,
                    Username = SiteUser.Username
                });
            }
            catch (Exception ex)
            {

                return Json(new { State = false, Message = ex.Message });
            }
        }


        [HttpPost]
        [AuthorizationFilter(Role = UserType.SysAdmin)]
        public JsonResult LicenceConfigEdit(string sysCode, string licenceNumber)
        {
            try
            {
                _business.LicenceConfigEdit(sysCode, licenceNumber);
                return Json(new
                {
                    State = true,
                    Message = "修改成功！",
                    Log = $" 用户 {SiteUser.Username} 修改配置: 授权码 为 “{sysCode}”，授权数 为 “{licenceNumber}”",
                    LogType = LogType.ConfigManage,
                    Username = SiteUser.Username
                });
            }
            catch (Exception ex)
            {
                return Json(new { State = false, Message = ex.Message });
            }
        }
    }
}

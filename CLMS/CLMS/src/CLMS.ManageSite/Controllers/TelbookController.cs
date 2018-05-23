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
    public class TelbookController : BaseController
    {
        private TelbookBusiness _business = new TelbookBusiness();
        // GET: /<controller>/

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Search(int limit ,int offset,string name, string telephone)
        {
            var list = _business.GetPage(name, telephone, limit, offset);
            return Json(list);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(TelbookModel model)
        {
            try
            {
                _business.Insert(model);
                return Json(new JsonResultLogModel
                {
                    State = true,
                    Message = "用户添加成功！",
                    Log = $" 用户 {SiteUser.Username} 添加人员 {model.Name} 成功  ",
                    LogType = LogType.TelbookManage,
                    Username = SiteUser.Username
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
        //[AuthorizationFilter(Role = UserType.SysAdmin)]
        public JsonResult Edit(TelbookModel model)
        {
            try
            {
                //model.Content.Replace("\r\n", "");                
                _business.Update(model);
                return Json(new JsonResultLogModel
                {
                    State = true,
                    Message = "用户信息修改成功！",
                    Log = $" 用户 {SiteUser.Username} 编辑人员 {model.Name} 成功  ",
                    LogType = LogType.TelbookManage,
                    Username = SiteUser.Username
                }); 
            }
            catch (Exception ex)
            {
                return Json(new { State = false, Message = ex.Message });
            }
        }

        [HttpPost]
        //[AuthorizationFilter(Role = UserType.SysAdmin)]
        public JsonResult Del(int[] ids,int? Id)
        {
            try
            {                
                if (Id.HasValue)                                 
                    _business.Del((int)Id);                
                else                
                    _business.Del(ids);
                return Json(new JsonResultLogModel
                {
                    State = true,
                    Message = "删除用户成功！",
                    Log = $" 用户 {SiteUser.Username} 删除人员 Id:{(Id.HasValue?Id.ToString():string.Join(",", ids))} 成功  ",
                    LogType = LogType.TelbookManage,
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

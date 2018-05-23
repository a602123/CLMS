using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using CLMS.Business;
using CLMS.Model;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CLMS.ManageSite.Controllers
{
    [ValidFilter]
    public class LineController : BaseController
    {
        private LineBusiness _business = new LineBusiness();

        // GET: /<controller>/
        public IActionResult Index(int? Id)
        {
            return View();
        }

        public JsonResult Search(int limit, int offset, string organizationName, string name, string lineIp)
        {
            var user = GetUser();
            var temp = _business.GetPage(organizationName, name, lineIp, user.OrganId, limit, offset);
            PageableData<object> list = new PageableData<object>()
            {
                total = temp.total,
                rows = temp.rows.Select(n => new
                {
                    n.Id,
                    n.Name,
                    n.LineIP,
                    n.Description,
                    LineType = n.LineType.GetDescription(),
                    ServiceProvider = n.ServiceProvider.GetDescription(),
                    n.OrganizationName,
                    n.ParentOrganizationName,
                    ConnectState = n.ConnectState ? "连通" : "断开"
                })
            };
            return Json(list);
        }

        public JsonResult GetLinesByOrganId(int organizationId)
        {
            var temp = _business.GetList(organizationId);
            var list = temp.Select(n => new
            {
                n.Id,
                n.Name,
                n.LineIP,
                n.Description,
                LineType = n.LineType.GetDescription(),
                ServiceProvider = n.ServiceProvider.GetDescription(),
                n.OrganizationName,
                n.ParentOrganizationName,
                ConnectState = n.ConnectState ? "连通" : "断开"
            });
            return Json(list);
        }

        public IActionResult Add(int id)
        {
            //var user = CookieHelper.GetInstance(HelperKeys.UserIdCookieKey).GetCookie<UserModel>(Request);
            var organ = new OrganBusiness().GetItem(id);
            LineModel model = new LineModel() { PingInterval = 30, Pingsize = 32, Pingtimes = 4, Timeout = 2, AlarmMax = 3 };
            if (organ!=null)
            {
                model.OrganizationId = organ.Id;
                model.OrganizationName = organ.Name;
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult Add(LineModel model)
        {
            try
            {
                LineModel addmodel = _business.Insert(model);
                MonitorCenter.GetInstance().Add(new List<LineModel> { addmodel });
                //return Json(new { State = true, Message = "用户添加成功！" });
                return Json( new 
                {
                    State = true,
                    Message = "用户添加成功！",
                    Log = $"用户 {GetUser().Username} 添加线路 {model.Name}",
                    LogType = LogType.LineManage,
                    GetUser().Username,
                    OrganId=addmodel.OrganizationId                 
                });
            }
            catch (Exception ex)
            {

                return Json(new { State = false, ex.Message });
            }
        }

        //public IActionResult AddTest()
        //{
        //    _business.InsertTest();
        //    return null;
        //}

        public IActionResult OrganAdd(int id)
        {
            //var user = GetUser();

            ViewBag.Organization = new OrganBusiness().GetItem(id);
            LineModel model = new LineModel() { PingInterval = 30, Pingsize = 32, Pingtimes = 4, Timeout = 2, AlarmMax = 3 };

            return View(model);
        }

        [HttpPost]
        public IActionResult OrganAdd(LineModel model)
        {
            try
            {
                LineModel addmodel = _business.Insert(model);
                MonitorCenter.GetInstance().Add(new List<LineModel> { addmodel });
                //return Json(new { State = true, Message = "用户添加成功！" });
                return Json(new JsonResultLogModel
                {
                    State = true,
                    Message = "用户添加成功！",
                    Log = $"用户 {GetUser().Username} 添加线路 {model.Name}",
                    LogType = LogType.LineManage,
                    Username = GetUser().Username
                });
            }
            catch (Exception ex)
            {

                return Json(new { State = false, ex.Message });
            }
        }

        public IActionResult Edit(int id)
        {
            var user = GetUser();
            //if (user.RoleId == UserType.SysAdmin)
            //{
            ViewBag.Organization = new OrganBusiness().GetParentNode();
            //}
            //else
            //{
            //    ViewBag.Organization = new OrganBusiness().GetItem(user.OrganId);
            //}

            var model = _business.GetItem(id);
            //ViewBag.Organization = new OrganBusiness().GetItem(model.OrganizationId);
            
            return View(model);
            
        }

        [HttpPost]
        //[AuthorizationFilter(Role = UserType.Admin | UserType.SysAdmin)]
        public JsonResult Edit(LineModel model)
        {
            try
            {
                //model.Content.Replace("\r\n", "");
                _business.Update(model);

                MonitorCenter.GetInstance().Remove(new List<int> { model.Id });
                MonitorCenter.GetInstance().Add(new List<LineModel> { model });
                return Json(new JsonResultLogModel
                {
                    State = true,
                    Message = "线路信息修改成功！",
                    Log = $" 用户 {GetUser().Username} 编辑线路 {model.Name} ",
                    LogType = LogType.LineManage,
                    Username = GetUser().Username
                });
            }
            catch (Exception ex)
            {
                return Json(new { State = false, Message = ex.Message });
            }
        }

        [HttpPost]
        //[AuthorizationFilter(Role = UserType.SysAdmin)]
        public JsonResult Del(int[] ids)
        {
            try
            {
                _business.Del(ids);
                MonitorCenter.GetInstance().Remove(new List<int> { ids.First() });
                return Json(new JsonResultLogModel
                {
                    State = true,
                    Message = "删除用户成功！",
                    Log = $" 用户 {GetUser().Username} 删除线路 Id:{string.Join(",", ids)} 成功  ",
                    LogType = LogType.LineManage,
                    Username = GetUser().Username
                });

            }
            catch (Exception ex)
            {
                return Json(new { State = false, Message = ex.Message });
            }
        }

        [AuthorizationFilter(Role = UserType.OrganAdmin | UserType.SysAdmin)]
        public IActionResult OrganLine(int id)
        {
            var user = GetUser();
            var organ = new OrganBusiness().GetItem(user.RoleId == UserType.OrganAdmin ? user.OrganId : id);
            return View(organ);
        }

        //联合查询
        public JsonResult GetList(int id)
        {
            try
            {
                var temp = new LineBusiness().GetList(id);
                List<object> list = new List<object>();
                foreach (var item in temp)
                {
                    list.Add(new
                    {
                        item.Id,
                        item.Name,
                        item.LineIP,
                        LineType = item.LineType.GetDescription(),
                        ServiceProvider = item.ServiceProvider.GetDescription(),
                        ConnectState = item.ConnectState ? "连通" : "断开",
                        item.Log,
                        CheckDate = item.CheckDate.ToString("yyyy-MM-dd HH:mm:ss")
                    });
                }
                return Json(list);
            }
            catch (Exception ex)
            {
                return Json(new { State = false,ex.Message });
            }
        }
    }
}

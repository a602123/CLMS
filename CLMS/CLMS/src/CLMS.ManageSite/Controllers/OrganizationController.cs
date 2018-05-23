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
    public class OrganizationController : BaseController
    {
        private OrganBusiness _business = new OrganBusiness();

        // GET: /<controller>/
        public IActionResult Index()
        {
            ViewBag.Initialize = _business.GetAllItems().Count == 0 ? true : false;
            return View();
        }

        public IActionResult Monitor()
        {
            return View();
        }

        public IActionResult OrganLineConfig()
        {
            ViewBag.style = "html height100%";
            return View();
        }
        [HttpPost]
        public JsonResult GetAllItems()
        {
            List<OrganModel> list = _business.GetChildren(GetUser().OrganId).OrderBy(n=>n.Id).ToList();//GetAllItems().Where(n=>n.ParentId==GetUser().OrganId).ToList();          
            return Json(list);
        }

        [AuthorizationFilter(Role = UserType.SysAdmin)]
        public ActionResult Add(int id)
        {
            OrganModel model = _business.GetItem(id);
            return View(new OrganModel() { ParentId = id, ParentOrganizationName = model == null ? "无" : model.Name });
        }

        [HttpPost]
        [AuthorizationFilter(Role = UserType.SysAdmin)]
        public JsonResult Add(OrganModel model)
        {
            try
            {
                //model.SMSTelephone= model.SMSTelephone.Substring(0, model.SMSTelephone.Length-1);
                _business.Insert(model);
                return Json(new JsonResultLogModel
                {
                    State = true,
                    Message = "组织添加成功！",
                    Log = $" 用户 {SiteUser.Username} 添加组织 {model.Name} ",
                    LogType = LogType.OrganManage,
                    Username = SiteUser.Username
                });
            }
            catch (Exception ex)
            {
                return Json(new { State = false, Message = ex.Message });
            }
        }

        [AuthorizationFilter(Role =  UserType.SysAdmin)]
        public ActionResult Edit(int id)
        {
            var model = _business.GetItem(id);
            ViewBag.SMSTelephone = model.SMSTelephone;
            model.ParentOrganizationName = model.ParentOrganizationName ?? "无";
            return View(model);
        }

        [HttpPost]
        [AuthorizationFilter(Role = UserType.SysAdmin)]
        public JsonResult Edit(OrganModel model)
        {
            try
            {
                if (model.Id == 1 || ViewDatas.GetInstance().GetUser(Request).RoleId != UserType.SysAdmin)
                {
                    return Json(new { State = false, Message = "您无权限编辑该组织！" });
                }
                else
                {
                    _business.Update(model);
                    return Json(new JsonResultLogModel
                    {
                        State = true,
                        Message = "组织信息修改成功！",
                        Log = $" 用户 {SiteUser.Username} 编辑组织 {model.Name} ",
                        LogType = LogType.OrganManage,
                        Username = SiteUser.Username
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new { State = false, Message = ex.Message });
            }
        }

        [HttpPost]
        [AuthorizationFilter(Role =  UserType.SysAdmin)]
        public JsonResult Del(int id)
        {
            try
            {
                _business.Del(id);
                return Json(new JsonResultLogModel
                {
                    State = true,
                    Message = "删除组织成功！",
                    Log = $" 用户 {SiteUser.Username} 删除组织 id:{id} ",
                    LogType = LogType.OrganManage,
                    Username = SiteUser.Username
                });
            }
            catch (Exception ex)
            {
                return Json(new { State = false, Message = ex.Message });
            }
        }


        public JsonResult GetListForSelect()
        {
            try
            {
                return Json(_business.GetListForSelect());
            }
            catch (Exception ex)
            {
                return Json(new { State = false, Message = ex.Message });
            }
        }


        public IActionResult ChooseOrgan()
        {
            return View();
        }

        public JsonResult GetChildren(int id)
        {
            return Json(_business.GetChildren(id));
        }

        [HttpPost]
        public JsonResult GetPosition()
        {
            try
            {
                var user = GetUser();
                List<OrganModel> list;
                //if (user.RoleId == UserType.SysAdmin)
                //{
                //    list = _business.GetPosition(null);
                //}
                //else
                //{
                //    list = _business.GetPosition(user.OrganId);
                //}
                list = _business.GetPosition(user.OrganId);
                return Json(new { State = true, Items = list });
            }
            catch (Exception ex)
            {
                return Json(new { State = false, Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult SetPosition(int id, string x, string y)
        {
            try
            {
                _business.SetPosition(id, x, y);
                return Json(new { State = true, Message = "位置设置成功！" });
            }
            catch (Exception ex)
            {
                return Json(new { State = false, Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetOrganLines(int id)
        {
            try
            {
                var itemOrgan = _business.GetItem(id);
                var temp = new LineBusiness().GetList(id);
                List<object> list = new List<object>();
                foreach (var item in temp)
                {
                    list.Add(new {
                        item.Id,
                        item.Name,
                        item.LineIP,
                        item.Description,
                        LineType = item.LineType.GetDescription(),
                        ServiceProvider = item.ServiceProvider.GetDescription(),
                        item.OrganizationName,
                        item.ParentOrganizationName,
                        ConnectState = item.ConnectState ? "连通" : "断开"
                    });
                }
                return Json(new { State = true, Items = list, Name = itemOrgan.Name, Des = itemOrgan.Description });
            }
            catch (Exception ex)
            {
                return Json(new { State = false, Message = ex.Message });
            }
        }

        public JsonResult GetOrganTree()
        {
            try
            {
                var user = GetUser();
                var result = _business.GetTreeView(user.OrganId);
                List<TreeViewModel> treeView = new List<TreeViewModel>();
                treeView.Add(result);
                return Json(treeView);
            }
            catch (Exception ex)
            {
                return Json(new { State = false, Message = ex.Message });
            }
        }

    }
}

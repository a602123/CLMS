using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CLMS.Model;
using CLMS.Business;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CLMS.ManageSite.Controllers
{
    public class AlarmController : BaseController
    {
        private AlarmBusiness _business = new AlarmBusiness();
        // GET: /<controller>/
        [AuthorizationFilter(Role = UserType.SysAdmin | UserType.OrganAdmin)]
        public IActionResult Index()
        {
            ViewBag.User = GetUser();
            return View();
        }

        [AuthorizationFilter(Role = UserType.SysAdmin | UserType.OrganAdmin)]
        public IActionResult RelatedInfo(int id)
        {
            ViewBag.Organ = new OrganBusiness().GetItem(id);
            return View();
        }

        [HttpPost]
        [AuthorizationFilter(Role = UserType.SysAdmin | UserType.OrganAdmin)]
        public JsonResult Search(int limit, int offset, string organizationName, string Name, string lineIP)
        {
            var user = CookieHelper.GetInstance(HelperKeys.UserIdCookieKey).GetCookie<UserModel>(Request);
            PageableData<AlarmModel> temp = _business.GetPage(Name, organizationName, lineIP, user.OrganId, limit, offset);

            PageableData<object> list = new PageableData<object>()
            {
                total = temp.total,
                rows = temp.rows.Select(n => new
                {
                    n.Id,
                    n.LineName,
                    n.IP,
                    n.OrganName,
                    FirstTime = n.FirstTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    LastTime = n.LastTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    n.AlarmCount,
                    State = n.State.GetDescription(),
                    Type = n.Type.GetDescription(),
                    n.Confirm
                    //LineState = n.LineState ? "连通" : "断开"
                })
            };

            return Json(list);
        }

        /// <summary>
        /// 现用作 线路 告警列表
        /// </summary>
        /// <param name="id">线路Id</param>
        /// <returns></returns>
        [AuthorizationFilter(Role = UserType.SysAdmin | UserType.OrganAdmin)]
        public IActionResult Log(int id)
        {
            ViewBag.Line = new LineBusiness().GetItem(id);
            return View();
        }

        [HttpPost]
        public JsonResult GetListByLine(int id)
        {
            try
            {
                var temp = _business.GetListByLine(id);
                List<object> list = new List<object>();
                foreach (var item in temp)
                {
                    list.Add(new
                    {
                        item.Id,
                        item.IP,
                        item.LineName,
                        item.OrganId,
                        Type = item.Type.GetDescription(),
                        FirstTime = item.FirstTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        LastTime = item.LastTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        item.AlarmCount,
                        item.Confirm,
                        State = item.State.GetDescription()
                        //ConnectState = item.LineState ? "连通" : "断开"
                    });
                }
                return Json(list);
            }
            catch (Exception ex)
            {
                return Json(new { State = false, Message = ex.Message });
            }
        }




        [AuthorizationFilter(Role = UserType.SysAdmin | UserType.OrganAdmin)]
        public JsonResult SolveLineAlarm(int lineId)
        {
            try
            {
                _business.SolveAlarmByLine(lineId);
                LogBusiness.GetInstance().Add(new LogModel() { Content = $" 用户 {ViewDatas.GetInstance().GetUser(Request).Username} 成功处理线路告警 线路Id：{lineId}", Time = DateTime.Now, Type = LogType.AlarmManage, Username = ViewDatas.GetInstance().GetUser(Request).Username });
                return Json(new { State = true, Message = "告警处理成功！" });
            }
            catch (Exception ex)
            {
                return Json(new { State = false, Message = ex.Message });
            }
        }


        [AuthorizationFilter(Role = UserType.SysAdmin | UserType.OrganAdmin)]
        public JsonResult SolveAlarm(string alarmId)
        {
            try
            {
                _business.SolveAlarm(alarmId);
                LogBusiness.GetInstance().Add(new LogModel() { Content = $" 用户 {ViewDatas.GetInstance().GetUser(Request).Username} 成功处理告警 Id：{alarmId}", Time = DateTime.Now, Type = LogType.AlarmManage, Username = ViewDatas.GetInstance().GetUser(Request).Username });
                return Json(new { State = true, Message = "告警处理成功！" });
            }
            catch (Exception ex)
            {
                return Json(new { State = false, Message = ex.Message });
            }
        }

        [HttpPost]
        [AuthorizationFilter(Role = UserType.SysAdmin | UserType.OrganAdmin)]
        public JsonResult SearchTop10()
        {
            var user = CookieHelper.GetInstance(HelperKeys.UserIdCookieKey).GetCookie<UserModel>(Request);
            IEnumerable<AlarmModel> temp;
            temp = _business.GetTop10(user.OrganId);


            List<object> list = new List<object>();
            foreach (var item in temp)
            {
                list.Add(new
                {
                    item.Id,
                    item.IP,
                    item.LineName,
                    item.OrganId,
                    Type = item.Type.GetDescription(),
                    FirstTime = item.FirstTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    LastTime = item.LastTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    item.AlarmCount,
                    item.Confirm,
                    State = item.State.GetDescription()
                    //ConnectState = item.LineState ? "连通" : "断开"
                });
            }

            return Json(list);
        }

        public JsonResult GetListInMap()
        {
            try
            {
                List<object> list = GetAlarmListWithinOrganValid();
                return Json(new { State = true, Items = list });
            }
            catch (Exception ex)
            {
                return Json(new { State = false, Message = ex.Message });
            }
        }

        public JsonResult GetList()
        {
            try
            {
                return Json(GetAlarmListWithinOrganValid());
            }
            catch (Exception ex)
            {
                return Json(new { State = false, Message = ex.Message });
            }
        }

        private List<object> GetAlarmListWithinOrganValid()
        {
            IEnumerable<AlarmModel> temp = _business.GetList(GetUser().OrganId);
            List<object> list = new List<object>();
            foreach (var item in temp)
            {
                list.Add(new
                {
                    item.Id,
                    item.IP,
                    item.LineName,
                    item.OrganName,
                    Type = item.Type.GetDescription(),
                    FirstTime = item.FirstTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    LastTime = item.LastTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    item.AlarmCount,
                    item.Confirm,
                    State = item.State.GetDescription(),
                    item.OrganId,
                    item.LineId
                    //ConnectState = item.LineState ? "连通" : "断开"
                });
            }
            return list;
        }
    }
}

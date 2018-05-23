using CLMS.Business;
using CLMS.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CLMS.ManageSite.Controllers
{
    public class BaseController: Controller
    {
        
        protected UserModel GetUser()
        {
             return CookieHelper.GetInstance(HelperKeys.UserIdCookieKey).GetCookie<UserModel>(Request);            
        }
    }
}

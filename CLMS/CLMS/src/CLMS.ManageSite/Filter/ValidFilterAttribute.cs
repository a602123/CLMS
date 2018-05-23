using CLMS.Business;
using CLMS.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CLMS.ManageSite
{
    public class ValidFilterAttribute : Attribute, IResultFilter
    {
        public UserType Role { get; set; }

        public void OnResultExecuted(ResultExecutedContext context)
        {
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            if (!LicenceBusiness.GetInstance().LicenceState)
            {
                context.Result = new RedirectResult("/Config/Index");
            }
        }
    }
}

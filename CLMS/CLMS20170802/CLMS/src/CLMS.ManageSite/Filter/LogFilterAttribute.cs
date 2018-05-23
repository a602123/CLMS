using CLMS.Model;
using CLMS.Business;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CLMS.ManageSite
{
    public class LogFilterAttribute : Attribute, IResultFilter
    {
        void IResultFilter.OnResultExecuted(ResultExecutedContext context)
        {
            
        }

        void IResultFilter.OnResultExecuting(ResultExecutingContext context)
        {
            
            var json = context.Result as JsonResult;
            if (json!=null)
            {
                var logResult = json.Value as JsonResultLogModel;                
                if (logResult != null)
                {
                    LogBusiness.GetInstance().Add(new LogModel() { Content = logResult.Log, Time = DateTime.Now, Type = logResult.LogType, Username = logResult.Username });                    
                    context.Result = new JsonResult(new { logResult.State, logResult.Message });
                }                
            }
        }
    }
}

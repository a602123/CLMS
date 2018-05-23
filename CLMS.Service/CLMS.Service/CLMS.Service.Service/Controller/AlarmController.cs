using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;

namespace CLMS.Service.Service.Controller
{
    public  class AlarmController:ApiController
    {
        [HttpGet]
        [Route("api/Alarm/get/{*id}")]
        public Dictionary<string,string> Get(int? id)
        {
            return new Dictionary<string, string>() {
                { "name", "11" }, { "age",id.ToString()}
            };
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CLMS.Model
{
    public class JsonResultLogModel : JsonResultBaseModel
    {
        public string Log { get; set; }

        public LogType LogType { get; set; }

        public string Username { get; set; }
    }
}

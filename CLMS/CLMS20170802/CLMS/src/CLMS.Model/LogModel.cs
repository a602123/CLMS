using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CLMS.Model
{
    /// <summary>
    /// 日志模型
    /// </summary>
    public class LogModel :JsonResultBaseModel
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime Time { get; set; }
        /// <summary>
        /// 日志类型
        /// </summary>
        public LogType Type { get; set; }
        public string Username { get; set; }
    }

}

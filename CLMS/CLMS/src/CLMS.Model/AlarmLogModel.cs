using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CLMS.Model
{
    /// <summary>
    /// 告警日志类型
    /// </summary>
    public class AlarmLogModel
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime Time { get; set; }
        public int AlarmId { get; set; }
    }
}

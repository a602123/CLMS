using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace CLMS.Model
{
    /// <summary>
    /// 告警类型
    /// </summary>
    public enum AlarmType
    {
        [Description("未知报警")]
        UnKnown = 0,
        [Description("线路没有连通")]
        LineNotConnected = 1
    }
}

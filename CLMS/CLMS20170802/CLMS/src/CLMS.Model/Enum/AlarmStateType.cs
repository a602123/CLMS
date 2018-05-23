using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace CLMS.Model
{
    /// <summary>
    /// 告警状态
    /// </summary>
    public enum AlarmStateType
    {
        [Description("错误的报警")]
        TempAlarm = 0,
        [Description("已恢复未确认")]
        Recover = 2,
        [Description("告警已关闭")]
        CloseAlarm = 1,
        [Description("正在告警")]
        OnAlarm = 3,
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace CLMS.Model
{
    /// <summary>
    /// 日志类型
    /// </summary>
    public enum LogType
    {
        [Description("未知日志")]
        Unknown,
        [Description("用户登录")]
        UserLogin,
        [Description("用户管理操作")]
        UserManager,
        [Description("组织管理操作")]
        OrganManage,
        [Description("线路管理操作")]
        LineManage,
        [Description("电话本管理操作")]
        TelbookManage,        
        [Description("告警管理操作")]
        AlarmManage,
        [Description("配置")]
        ConfigManage,
    }
}

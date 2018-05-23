using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace CLMS.Model
{
    [Flags]
    public enum UserType
    {
        [Description("系统管理员")]
        SysAdmin = 1,
        //[Description("管理员")]
        //Admin = 2,
        [Description("部门管理员")]
        OrganAdmin = 4
        //[Description("一般用户")]
        //Normal = 8
    }
}

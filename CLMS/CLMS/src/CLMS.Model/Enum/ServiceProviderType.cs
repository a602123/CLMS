using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace CLMS.Model
{
    [Flags]
    public enum ServiceProviderType
    {
        [Description("联通")]
        ChinaUnicom=1,
        [Description("移动")]
        ChinaMobile =2,
        [Description("电信")]
        ChinaTelecom =3,
        [Description("其他")]
        Orther =4
    }
}

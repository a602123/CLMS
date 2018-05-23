using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace CLMS.Model
{
    [Flags]
    public enum LineType
    {
        [Description("主线")]
        mainline=1,
        [Description("备线")]
        backupline=2

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CLMS.Model
{
    public static class EnumHelper
    {
        public static string GetDescription(this Enum item)
        {
            string strValue = item.ToString();
            FieldInfo fieldinfo = item.GetType().GetField(strValue);
            var objs = fieldinfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (objs == null || objs.Count() == 0)
            {
                return strValue;
            }
            else
            {
                DescriptionAttribute da = (DescriptionAttribute)objs.First();
                return da.Description;
            }

        }        
    }    
}

using CLMS.DataProvider;
using CLMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CLMS.Business
{
    /// <summary>
    /// 业务数据的配置
    /// </summary>
    public class ConfigBusiness
    {
        private SysConfigDataProvider _sysprovider = new SysConfigDataProvider();

        public Dictionary<string, string> GetConfigList()
        {
            Dictionary<string, string> dic = GetConfigFromDB("");
            //dic.Add("MachineCode", LicenceBusiness.GetInstance().MachineCode);
            //dic.Add("LicenceState", LicenceBusiness.GetInstance().LicenceState ? "通过授权" : "未通过授权");
            return dic;
        }

        internal Dictionary<string, string> GetConfigFromDB(string condition)
        {
            return _sysprovider.GetConfig(condition);
        }

        public void SysConfigEdit(string sysName, string rootName, string defaultPassword)
        {
            Dictionary<string, string> updates = new Dictionary<string, string>();
            updates.Add("SysName", sysName);
            updates.Add("RootName", rootName);
            updates.Add("DefaultPassword", defaultPassword);
            _sysprovider.Update(updates);

            //同时修改组织机构
            //new OrganBusiness().Update(new OrganModel() { Id=1, Description = rootName, Name = rootName, State = 1, ParentId = 0 });
            var model = new OrganBusiness().GetParentNode();
            if (model != null)
            {
                model.Name = rootName;
                model.Description = rootName;
                new OrganBusiness().Update(model);
            }
        }

        public void PingConfigEdit(string interval, string pingtimes, string pingsize, string pingttl)
        {
            Dictionary<string, string> updates = new Dictionary<string, string>();
            updates.Add("Interval", interval);
            updates.Add("Pingtimes", pingtimes);
            updates.Add("Pingsize", pingsize);
            updates.Add("Pingttl", pingttl);
            _sysprovider.Update(updates);
            
            //改变IPping的以上参数参数    
        }
    }
}

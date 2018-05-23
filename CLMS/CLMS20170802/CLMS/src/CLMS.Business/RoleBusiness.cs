using CLMS.DataProvider;
using CLMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CLMS.Business
{
    public class RoleBusiness
    {
        private RoleDataProvider _provider;

        public RoleBusiness()
        {
            _provider = RoleDataProvider.GetInstance();
        }

        public List<RoleModel> GetList()
        {
            return _provider.GetList();
        }
    }
}

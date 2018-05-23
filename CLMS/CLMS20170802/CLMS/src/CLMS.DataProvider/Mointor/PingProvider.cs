using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CLMS.DataProvider
{
    public class PingProvider : BaseProvider 
    {
        public PingProvider(string tableName = "tb_switch") : base(tableName)
        {

        }

        protected override string InsertStr
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}

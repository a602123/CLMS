using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CLMS.Model
{
    public class PageableData<T>
    {
        public int total { get; set; }
        public IEnumerable<T> rows { get; set; }
    }
}

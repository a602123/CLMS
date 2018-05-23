using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CLMS.Model
{
    public class OrganModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ParentId { get; set; }
        public string ParentName { get; set; }
        public string ParentOrganizationName { get; set; }
        public int State { get; set; }
        public string SMSTelephone { get; set; }
        public string X { get; set; }
        public string Y { get; set; }
        public int LineCount { get; set; }

    }
}

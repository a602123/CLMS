using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CLMS.Model
{
    public class LineModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LineIP { get; set; }
        public string Description { get; set; }
        public LineType LineType { get; set; }
        //public string LineTypeName { get; set; }
        public ServiceProviderType ServiceProvider { get; set; }
        //public string ServiceProviderName { get; set; }
        public int OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public string OrganizationDescription { get; set; }
        public string ParentOrganizationName { get; set; }
        /// <summary>
        /// Ping包的大小,默认32字节
        /// </summary>
        public int Pingsize { get; set; }
        /// <summary>
        /// 轮询ping的次数，默认4次
        /// </summary>
        public int Pingtimes { get; set; }
        /// <summary>
        /// 默认超时时间，2 秒
        /// </summary>
        public int Timeout { get; set; }
        /// <summary>
        /// 轮询周期，默认30秒
        /// </summary>
        public int PingInterval { get; set; }

        /// <summary>
        /// 实时的线路通断状态
        /// </summary>
        public bool ConnectState { get; set; }

        public object Log { get; set; }

        public int AlarmMax { get; set; }

        public string SMSTelphone { get; set; }

        public DateTime CheckDate { get; set; }
    }
}

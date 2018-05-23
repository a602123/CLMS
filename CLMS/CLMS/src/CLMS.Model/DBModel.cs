using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CLMS.Model
{
    /// <summary>
    /// 数据库连接信息模型
    /// </summary>
    public class DBModel
    {
        public int Id { get; set; }
        /// <summary>
        /// 数据库Id 如sa root
        /// </summary>
        public string DBId { get; set; }
        /// <summary>
        /// 数据库登陆密码
        /// </summary>
        public string DBPassword { get; set; }
        /// <summary>
        /// 数据库名称 db_ipms
        /// </summary>
        public string DBSource { get; set; }
        /// <summary>
        /// 数据库所在IP 192.168.1.234
        /// </summary>
        public string DBHost { get; set; }
        /// <summary>
        /// 数据库连接信息模型 名称
        /// </summary>
        public string Name { get; set; }

        public string Note { get; set; }
        /// <summary>
        /// 是否为默认数据库
        /// </summary>
        public int IsDefault { get; set; }
    }
}

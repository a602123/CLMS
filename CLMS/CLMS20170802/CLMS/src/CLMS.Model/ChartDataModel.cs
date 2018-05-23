using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CLMS.Model
{
    /// <summary>
    /// 用于统计 chart.js 所需的数据模型
    /// </summary>
    public class ChartDataModel
    {
        /// <summary>
        /// 每个节点的标签内容集合
        /// </summary>
        public List<string> Titles { get; set; }
        /// <summary>
        /// 每个节点的 数值 集合
        /// </summary>
        public List<int> Datas { get; set; }
    }

    /// <summary>
    /// 为了Json读取的临时Model
    /// </summary>
    public class StatisticsTempModel
    {
        public string Name { get; set; }
        public ChartDataModel Data { get; set; }
    }
}

using System;
using System.Xml.Serialization;

namespace Gwsoft.DataSpec2
{
    /// <summary>
    /// 数据项结构
    /// </summary>
    [Serializable]
    [System.Diagnostics.DebuggerDisplay("{DataName,nq} = {ItemValue,nq} [{DataType,nq}]")]
    public struct DataItem
    {
        /// <summary>
        /// 传输序号(没有设置则按数据在容器中的排序)
        /// </summary>
        [XmlAttribute]
        public int? ExchangeIndex { get; set; }

        /// <summary>
        /// 是否是网络字节序(默认为true)
        /// </summary>
        [XmlAttribute]
        public bool NetworkBytes { get; set; }

        /// <summary>
        /// 数据类型
        /// </summary>
        [XmlAttribute]
        public string DataType { get; set; }

        /// <summary>
        /// 数据项描述
        /// </summary>
        [XmlAttribute]
        public string Description { get; set; }

        /// <summary>
        /// 数据项命名
        /// </summary>
        [XmlAttribute]
        public string DataName { get; set; }

        /// <summary>
        /// 该数据项上绑定的数据
        /// </summary>
        [XmlIgnore]
        public object ItemValue { get; set; }

        /// <summary>
        /// 传输的条件表达式
        /// </summary>
        public string ConditionalExpression { get; set; }
    }
}

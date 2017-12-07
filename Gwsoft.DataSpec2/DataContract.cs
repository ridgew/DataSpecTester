using System;
using System.Xml.Serialization;

namespace Gwsoft.DataSpec2
{
    /// <summary>
    /// 数据传输契约
    /// </summary>
    [Serializable]
    [System.Diagnostics.DebuggerDisplay("{ContractName,nq}")]
    public class DataContract : ISpecObject
    {
        /// <summary>
        /// 获取或设置契约名称
        /// </summary>
        [XmlAttribute]
        public string ContractName { get; set; }

        string _cmp = null;
        /// <summary>
        /// 协议兼容型及版本, 例：Ease/3.2
        /// </summary>
        [XmlAttribute]
        public string Compatibility
        {
            get { return _cmp; }
            set { _cmp = value; }
        }

        string _cmpRef = null;
        /// <summary>
        /// 数据结构兼容协议参考地址(可选)
        /// <para>例：http://server.ease/contracts/ease/index.shtml</para>
        /// </summary>
        [XmlAttribute]
        public string Compatibility_Reference
        {
            get { return _cmpRef; }
            set { _cmpRef = value; }
        }

        int _itemCount = 1;
        /// <summary>
        /// 总共配置项，自1开始。(至少有一个配置项)
        /// </summary>
        [XmlAttribute]
        public int ConfigItemCount
        {
            get { return _itemCount; }
            set { _itemCount = value; }
        }

        DataItem[] _transItems = new DataItem[0];
        /// <summary>
        /// 该数据协议传输的所有数据项
        /// </summary>
        public DataItem[] TransItems
        {
            get { return _transItems; }
            set { _transItems = value; }
        }
    }
}

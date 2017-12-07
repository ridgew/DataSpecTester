using System;
using System.Collections.Generic;
using System.Text;

namespace Gwsoft.DataSpec
{
    /// <summary>
    /// 对象内数据存放顺序
    /// </summary>
    public interface IDataIndex
    {
        /// <summary>
        /// 数据存放位置索引的开始与结束位置(长度为2)
        /// </summary>
        long[] MappingRange { get; set; }

        /// <summary>
        /// 获取或设置当前对象是否只绑定了索引映射位置,而没有绑定内容数据.
        /// </summary>
        bool JustMapped { get; set; }
    }

    /// <summary>
    /// 实现字节序列的转换传输
    /// </summary>
    public interface IByteTransfer
    {
        /// <summary>
        /// 获取主机字节序列
        /// </summary>
        byte[] GetHostBytes();

        /// <summary>
        /// 获取网络字节序列
        /// </summary>
        byte[] GetNetworkBytes();

        /// <summary>
        /// 是否已实现字节数据绑定
        /// </summary>
        bool HasImplementDataBind { get; }

        /// <summary>
        /// 从网络字节序列中加载数据
        /// </summary>
        void LoadFromNetworkBytes(byte[] networkBytes);
    }
}

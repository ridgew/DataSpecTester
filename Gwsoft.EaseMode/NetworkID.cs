using System;
using System.Collections.Generic;
using System.Text;

namespace Gwsoft.EaseMode
{
    /// <summary>
    /// 网络运营商ID(byte)
    /// </summary>
    public enum NetworkID : byte
    {
        /// <summary>
        /// 全部
        /// </summary>
        ALL = 0,
        /// <summary>
        /// 中国移动
        /// </summary>
        CMCC = 1,
        /// <summary>
        /// 中国联通
        /// </summary>
        CUC = 2,
        /// <summary>
        /// 中国电信
        /// </summary>
        CTG = 3,
    }

    /// <summary>
    /// 客户端拨号方式(byte)
    /// </summary>
    public enum ClientDialType : byte
    {
        /// <summary>
        /// 互联网关 = 0
        /// </summary>
        CTNET = 0,
        /// <summary>
        /// WAP网关 = 1
        /// </summary>
        CTWAP = 1
    }
}

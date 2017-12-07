using System;
using System.Collections.Generic;
using System.Text;

namespace Gwsoft.EaseMode
{
    /// <summary>
    /// 编码序号(byte)
    /// </summary>
    public enum EaseEncode : byte
    {
        /// <summary>
        /// utf-8
        /// </summary>
        UTF8 = 0,
        /// <summary>
        /// unicode
        /// </summary>
        Unicode = 1,
        /// <summary>
        /// 国标2312字符集
        /// </summary>
        GB2312 = 2
    }

    /// <summary>
    /// EASE成功标识(short)
    /// </summary>
    public enum EaseSuccessFlag : short
    { 
        /// <summary>
        /// 通用失败 = -1
        /// </summary>
        Error = -1,

        /// <summary>
        /// 未知标识
        /// </summary>
        UnKnown = 0,

        /// <summary>
        /// 网络连接成功(完整版）= 1010
        /// </summary>
        Success = 1010,

        /// <summary>
        /// 网络连接成功(简易版）= 1120
        /// </summary>
        SuccessExpress = 1120,
    }

    /// <summary>
    /// EASE数据压缩格式(byte)
    /// (0－不压缩 1－lz77压缩算法)
    /// </summary>
    public enum EaseCompress : byte
    { 
        /// <summary>
        /// 不压缩 = 0
        /// </summary>
        NoCompress = 0,

        /// <summary>
        /// lz77压缩算法 = 1
        /// </summary>
        Lz77 = 1
    }

    /// <summary>
    /// 首次连网标志(1-开机 0-使用中 2-清掉缓存)
    /// </summary>
    public enum EaseConnectState : byte
    { 
        /// <summary>
        /// 工作中 = 0
        /// </summary>
        Working = 0,
        /// <summary>
        /// 开机 = 1
        /// </summary>
        StartUp = 1,
        /// <summary>
        /// 清掉缓存 = 2
        /// </summary>
        ClearCache = 2
    }
}

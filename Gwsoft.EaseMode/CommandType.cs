using System;
using System.Collections.Generic;
using System.Text;

namespace Gwsoft.EaseMode
{
    /// <summary>
    /// 操作类型(byte)
    /// </summary>
    public enum CommandType : byte
    {
        /// <summary>
        /// 未知
        /// </summary>
        UnKnown = 0,

        /// <summary>
        /// 无任何操作 = 1
        /// </summary>
        None = 1,
        /// <summary>
        /// 发送短信 = 2
        /// </summary>
        SMS = 2,
        /// <summary>
        /// 调用WAP浏览器 = 3
        /// </summary>
        WAP = 3,
        /// <summary>
        /// 拨打电话 = 4
        /// </summary>
        Dial = 4,
        /// <summary>
        /// 主程序存在更新，下载主程序 = 5
        /// </summary>
        Updatable = 5
    }
}

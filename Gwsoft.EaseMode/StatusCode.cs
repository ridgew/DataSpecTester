using System;
using System.Collections.Generic;
using System.Text;
using Gwsoft.Configuration;

namespace Gwsoft.EaseMode
{
    /// <summary>
    /// 服务器响应码(short 3.2-2.6.9)
    /// </summary>
    [ImplementState(CompleteState.OK, "1.0(v3.2-2.6.9)", Description = "EASE网络接入服务器响应码", ReleaseDateGTM = "Mon, 04 Jan 2010 13:06:34 GMT")]
    public enum StatusCode : short
    {
        /// <summary>
        /// 服务器异常 = -1
        /// </summary>
        Exception = -1,

        /// <summary>
        /// 服务器处理成功 = 0
        /// </summary>
        Success = 0,

        /// <summary>
        /// 主程序有更新,请按照客户端更新策略下载 = 1
        /// </summary>
        Updatable = 1
    }
}

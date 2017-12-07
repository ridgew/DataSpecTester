using System;
using System.Collections.Generic;
using System.Text;

namespace Gwsoft.EaseMode
{
    /// <summary>
    /// 客户端功能号(byte)
    /// </summary>
    public enum RequestType : byte
    {
        /// <summary>
        /// 页面请求（兼容EASE 2.1）= 0
        /// </summary>
        PageV21 = 0,
        /// <summary>
        /// 页面及资源请求 = 1
        /// </summary>
        Mixed  = 1,
        /// <summary>
        /// 页面请求 = 2
        /// </summary>
        Page = 2,
        /// <summary>
        /// 资源请求 = 3
        /// </summary>
        Resource = 3,
        /// <summary>
        /// 应用请求 = 4
        /// </summary>
        Application = 4,
        /// <summary>
        /// 更新服务器连接地址 = 5
        /// </summary>
        UpdateCenter = 5
    }
}

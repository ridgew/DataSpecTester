using System;
using System.Collections.Generic;
using System.Text;
using Gwsoft.DataSpec;

namespace Gwsoft.EaseMode
{
    /// <summary>
    /// 所有响应基类
    /// </summary>
    public abstract class ResponseBase : ESPDataBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseBase"/> class.
        /// </summary>
        public ResponseBase()
            : base()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseBase"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public ResponseBase(ESPContext context)
            : base(context)
        { }

        #region 属性模型顺序
        /// <summary>
        /// 响应头信息
        /// </summary>
        [ObjectTransferOrder(0, Reverse = false, Offset = 0)]
        public ResponseHeader ESP_Header { get; set; }

        /// <summary>
        /// 服务器端响应码(响应码不为0时弹出提示框)
        /// </summary>
        [ObjectTransferOrder(1, Reverse = true, Offset = -1)]
        public StatusCode ESP_Code { get; set; }

        /// <summary>
        /// 响应码不为0 时弹出提示框的消息
        /// </summary>
        [ObjectTransferOrder(2, Reverse = false, Offset = 2)]
        public EaseString ESP_Message { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        [ObjectTransferOrder(3, Reverse = false, Offset = -1)]
        public CommandType ESP_Method { get; set; }

        /// <summary>
        /// 操作指令
        /// <para>操作类型为0 时不存在，长度为0</para>
        /// <para>操作类型为1 时为短信指令</para>
        /// <para>操作类型为2 时为WAP 链接</para>
        /// <para>操作类型为3 时为电话号码</para>
        /// <para>操作类型为4 时为主程序下载链接</para>
        /// </summary>
        [ObjectTransferOrder(4, Reverse = false, Offset = 1)]
        public EaseString ESP_Command { get; set; }
        #endregion

    }
}

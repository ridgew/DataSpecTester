using Gwsoft.DataSpec;

namespace Gwsoft.EaseMode
{
    /// <summary>
    /// 所有请求基类(包含请求头信息配置)
    /// </summary>
    public abstract class RequestBase : ESPDataBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestBase"/> class.
        /// </summary>
        public RequestBase()
            : base()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestBase"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public RequestBase(ESPContext context)
            : base(context)
        { }

        #region 属性模型顺序
        /// <summary>
        /// 请求头信息
        /// </summary>
        [ObjectTransferOrder(0, Reverse = false, Offset = 0)]
        public RequestHeader ESP_Header { get; set; }
        #endregion
    }
}

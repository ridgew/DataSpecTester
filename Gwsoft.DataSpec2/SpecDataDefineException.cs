using System;

namespace Gwsoft.DataSpec2
{
    /// <summary>
    /// 规则数据定义错误异常
    /// </summary>
    public class SpecDataDefineException : ApplicationException
    {
        /// <summary>
        /// 初始化 <see cref="SpecDataDefineException"/> class.
        /// </summary>
        public SpecDataDefineException()
            : base()
        {

        }

        /// <summary>
        /// 初始化一个 <see cref="SpecDataDefineException"/> class 实例。
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public SpecDataDefineException(string message, Exception innerException)
            : base(message, innerException)
        { }

        /// <summary>
        /// 初始化一个 <see cref="SpecDataDefineException"/> class 实例。
        /// </summary>
        /// <param name="message">The message.</param>
        public SpecDataDefineException(string message)
            : base(message)
        { }
    }
}

using System;
using System.Collections.Generic;

namespace Gwsoft.DataSpec
{
    /// <summary>
    /// 无效的规范数据异常
    /// </summary>
    public class BadSpecDataException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BadSpecDataException"/> class.
        /// </summary>
        public BadSpecDataException()
            : base()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BadSpecDataException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public BadSpecDataException(string message)
            : base(message)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BadSpecDataException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public BadSpecDataException(string message, Exception innerException)
            : base(message, innerException)
        { }


        private List<BadSpecDataException> _childExpList = new List<BadSpecDataException>();
        /// <summary>
        /// 所有子级异常
        /// </summary>
        public List<BadSpecDataException> ChildException
        {
            get { return _childExpList; }
        }

        /// <summary>
        /// 添加子级异常对象
        /// </summary>
        public BadSpecDataException WithChild(BadSpecDataException childSpecException)
        {
            _childExpList.Add(childSpecException);
            return this;
        }

    }
}

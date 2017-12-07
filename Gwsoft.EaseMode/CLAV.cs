using System;
using System.Collections.Generic;
using System.Text;

namespace EaseServer.ESP
{
    /// <summary>
    /// 不定长度ASCII字符串
    /// </summary>
    public class CLAV : CLUV
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="CLAV"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public CLAV(ESPContext context)
            : base(context)
        { }

        /// <summary>
        /// 获取不定长字符串(ASCII编码)的实际内容
        /// </summary>
        /// <returns></returns>
        public override string GetContent()
        {
            if (_sBytes.Length > 0)
            {
                return Encoding.ASCII.GetString(_sBytes);
            }
            else
            {
                return string.Empty;
            }
        }
    }

    /// <summary>
    /// 不定长度UTF-8字符串
    /// </summary>
    public class CLTV : CLUV
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CLUV"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public CLTV(ESPContext context)
            : base(context)
        { }

        /// <summary>
        /// 获取不定长字符串(ASCII编码)的实际内容
        /// </summary>
        /// <returns></returns>
        public override string GetContent()
        {
            if (_sBytes.Length > 0)
            {
                return Encoding.UTF8.GetString(_sBytes);
            }
            else
            {
                return string.Empty;
            }
        }
    }
}

using System;

namespace Gwsoft.DataSpec
{
    /// <summary>
    /// 定义对象传输的关联属性的自动设置
    /// </summary>
    public interface IAutoSetter
    {
        /// <summary>
        /// 在当前上下文中设置自身其他属性
        /// </summary>
        void AutoSetter(ESPContext context);
    }
}

using System;

namespace Gwsoft.DataSpec
{
    /// <summary>
    /// 对象属性网络传输的顺序
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class ObjectTransferOrderAttribute : Attribute
    {

        /// <summary>
        /// 初始化一个 <see cref="ObjectTransferOrderAttribute"/> class 实例。
        /// </summary>
        /// <param name="order">排序设置，值越小越靠前。0为起始索引排序。</param>
        public ObjectTransferOrderAttribute(int order)
        {
            Order = order;
        }

        private int _order = 0;
        /// <summary>
        /// 排序位置,以0开始.
        /// </summary>
        public int Order 
        {
            get { return _order; }
            set { _order = value; }
        }

        /// <summary>
        /// 从级排序(单精度浮点数)，用于在主级排序冲突时备用排序参数。
        /// </summary>
        public float SubOrder { get; set; }

        /// <summary>
        /// 传输之前是否反转字节序列
        /// </summary>
        public bool Reverse { get; set; }

        private long _offsetDefault = -1L;
        /// <summary>
        /// 相对于上一个传输属性的起始字节相对偏移量(默认为-1,需要动态计算)
        /// </summary>
        public long Offset
        {
            get { return _offsetDefault; }
            set { _offsetDefault = value; }
        }

        private long _runtimeOffset = -1L;
        /// <summary>
        /// 运行时当前属性在整个流中的偏移量(动态修改)
        /// </summary>
        public long RuntimeOffset
        {
            get { return _runtimeOffset; }
            set { _runtimeOffset = value; }
        }

        //private Type _nestedType = null;
        ///// <summary>
        ///// 对于字节序列所表示的嵌套类型
        ///// </summary>
        //public Type NestedType
        //{
        //    get { return _nestedType; }
        //    set { _nestedType = value; }
        //}

        private bool _transConditional = false;
        /// <summary>
        /// 当前属性是否在在满足条件下传输(默认为false)，如为true则需要实现自定义实例属性绑定。
        /// </summary>
        public bool Conditional
        {
            get { return _transConditional; }
            set { _transConditional = value; }
        }

        private int _arrayLengthOffset = -1;
        /// <summary>
        /// 如果传输对象为数组，则数组长度配置属性偏移量为。（默认-1）
        /// </summary>
        public int ArrayLengthOffset
        {
            get { return _arrayLengthOffset; }
            set { _arrayLengthOffset = value; }
        }

        private string _description = string.Empty;
        /// <summary>
        /// 描述说明
        /// </summary>
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

    }
}

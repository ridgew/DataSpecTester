using System;

namespace DataSpecTester
{
    /// <summary>
    /// 当前操作界面的视图状态
    /// </summary>
    public enum InterfaceViewMode : int
    {

        /// <summary>
        /// 数据抓包视图
        /// </summary>
        CapDateView = 0,

        /// <summary>
        /// 二进制数据视图
        /// </summary>
        BinDataView = 1,

        /// <summary>
        /// 请求数据视图
        /// </summary>
        RequestView = 2,

        /// <summary>
        /// 其他数据视图
        /// </summary>
        ResponseView = 3,

        /// <summary>
        /// 其他数据视图
        /// </summary>
        OtherView = 4

    }
}

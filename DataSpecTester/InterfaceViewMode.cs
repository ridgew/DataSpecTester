using System;

namespace DataSpecTester
{
    /// <summary>
    /// 当前操作界面的视图状态
    /// </summary>
    public enum InterfaceViewMode : int
    {

        /// <summary>
        /// 请求数据视图
        /// </summary>
        RequestView = 0,

        /// <summary>
        /// 返回数据视图
        /// </summary>
        ResponseView = 1,

        /// <summary>
        /// 其他数据视图
        /// </summary>
        OtherView = 2

    }
}

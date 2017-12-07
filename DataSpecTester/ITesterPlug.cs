using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace DataSpecTester
{
    /// <summary>
    /// 消息输出控制委托
    /// </summary>
    public delegate void LogWriter(string logFormat, params object[] args);

    /// <summary>
    /// 测试插件接口
    /// </summary>
    public interface ITesterPlug
    {
        /// <summary>
        /// 获取插件宿主内的子类型集合
        /// </summary>
        /// <param name="baseType">基类型</param>
        /// <returns></returns>
        Type[] GetPlugHostTypes(Type baseType);

        /// <summary>
        /// 显示选择树中特定节点的详细信息，并返回在数据源中的索引起始位置
        /// </summary>
        long[] ReportNodeDetail(TreeView targetTree, TreeNode selectNode, LogWriter rptWriter);

        /// <summary>
        /// 修改绑定树的实例之后更新绑定
        /// </summary>
        void RefreshTreeView(TreeView targetTree, LogWriter rptWriter);

        /// <summary>
        /// 修改节点绑定之后的更新
        /// </summary>
        void RefreshSelectNode(TreeNode selectNode, LogWriter rptWriter);

        /// <summary>
        /// 解析二进制数据为树形数据查看
        /// </summary>
        /// <param name="espDataBin">协议二进制数据</param>
        /// <param name="tvDisplay">对象解析树</param>
        /// <param name="writer">日志输出</param>
        /// <returns>协议详细封装类型名称</returns>
        bool Parse(byte[] espDataBin, TreeView tvDisplay, LogWriter writer, out string returnType);

        /// <summary>
        /// 尝试以制定类型解析二进制序列对象
        /// </summary>
        /// <param name="espDataBin">协议二进制数据</param>
        /// <param name="typeName">类型名称</param>
        /// <param name="tvDisplay">对象解析树</param>
        /// <param name="writer">日志输出</param>
        bool TryParseAsType(byte[] espDataBin, string typeName, TreeView tvDisplay, LogWriter writer);

        /// <summary>
        /// 获取远程测试返回数据
        /// </summary>
        byte[] GetTestResponse(byte[] espRequestData, LogWriter writer);

        /// <summary>
        /// 设置跟踪输出口
        /// </summary>
        void SetTraceWriter(LogWriter writer);

        /// <summary>
        /// 为交互应用保存对象
        /// </summary>
        /// <param name="objKey">对象的键值</param>
        /// <param name="storeObj">保存对象的值</param>
        void ExchangeStore(string objKey, object storeObj);

        /// <summary>
        /// 为交互应用获取对象
        /// </summary>
        /// <param name="objKey">获取对象的键值</param>
        /// <returns></returns>
        object ExchangeGet(string objKey);

    }

    /// <summary>
    /// 图像列表的枚举表示
    /// </summary>
    public enum ImageListIcon : int
    {
        Class  = 0,
        Constant = 1,
        Enum = 2,
        EnumItem = 3,
        ExtensionMethod = 4,
        Field = 5,
        Method = 6,
        Property = 7,
        Struct = 8
    }

    /// <summary>
    /// Treeview节点绑定对象
    /// </summary>
    public class TreeNodeInstanceBind
    {
        /// <summary>
        /// 节点对象
        /// </summary>
        public object NodeItem { get; set; }
        /// <summary>
        /// 是否是第一个节点
        /// </summary>
        public bool IsFirstNode { get; set; }
        /// <summary>
        /// 是否是ESP对象封装
        /// </summary>
        public bool IsESPData { get; set; }
        /// <summary>
        /// 顶层容器中的开始索引
        /// </summary>
        public long StoreIndex { get; set; }
        /// <summary>
        /// 存储数据长度
        /// </summary>
        public long StoreLength { get; set; }
        /// <summary>
        /// 是否是数组元素项
        /// </summary>
        public bool IsArrayItem { get; set; }

        private bool _tagModified = false;
        /// <summary>
        /// 获取或设置绑定的标签是否已修改(默认否)
        /// </summary>
        public bool TagModified
        {
            get { return _tagModified; }
            set { _tagModified = value; }
        }

        /// <summary>
        /// 节点数据类型
        /// </summary>
        public Type NodeType { get; set; }

        /// <summary>
        /// 从属类型
        /// </summary>
        public Type SubType { get; set; }
    }
}

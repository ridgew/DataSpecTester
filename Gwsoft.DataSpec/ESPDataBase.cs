using System;
using System.IO;
using System.Reflection;

namespace Gwsoft.DataSpec
{
    /// <summary>
    /// 基于ESP协议的网络数据传输基类(自身无传输属性配置）
    /// </summary>
    [Serializable]
    public abstract class ESPDataBase : IByteTransfer
    {
        /// <summary>
        /// 初始化 <see cref="ESPDataBase"/> class.
        /// </summary>
        public ESPDataBase()
        {
            CustomPropertyBindAction();
        }

        /// <summary>
        /// 初始化一个 <see cref="ESPDataBase"/> class 实例。
        /// </summary>
        /// <param name="context">The context.</param>
        public ESPDataBase(ESPContext context)
        {
            CustomPropertyBindAction();
            Context = context;
        }

        /// <summary>
        /// 获取字节序列总长度
        /// </summary>
        public virtual long GetContentLength()
        {
            return _mapRng[1] - _mapRng[0] + 1;
        }

        #region IByteTransfer 成员
        /// <summary>
        /// 是否内部已实现数据绑定(默认为false)，通常自实现绑定的已实现序列化。
        /// </summary>
        protected bool internalImplementDataBind = false;

        /// <summary>
        /// 是否已实现字节数据绑定
        /// </summary>
        /// <value></value>
        public bool HasImplementDataBind
        {
            get { return internalImplementDataBind; }
        }


        /// <summary>
        /// 获取主机字节序列 == 获取网络字节序列
        /// </summary>
        /// <returns></returns>
        public virtual byte[] GetHostBytes()
        {
            return GetNetworkBytes();
        }

        /// <summary>
        /// 获取网络字节序列
        /// </summary>
        /// <returns></returns>
        public virtual byte[] GetNetworkBytes()
        {
            return GetInstanceNetworkBytes(null);
        }

        /// <summary>
        /// 获取配置成员属性的网络传输字节序列
        /// </summary>
        /// <param name="skipPropertyFn">相关属性是否忽略的判断委托</param>
        /// <returns></returns>
        public byte[] GetInstanceNetworkBytes(Func<PropertyInfo, bool> skipPropertyFn)
        {
            //获取需要传输的字节队列
            //依据传输规则封装
            using (MemoryStream ms = new MemoryStream())
            {
                Type InstanceType = this.GetType();

                ObjectTransferOrderAttribute[] transConfig = new ObjectTransferOrderAttribute[0];
                PropertyInfo[] transPropertys = SpecUtil.GetTransferProperties(InstanceType, out transConfig);

                byte[] currentBytes = new byte[0];
                ObjectTransferOrderAttribute currentConfig = null;
                PropertyInfo pInfo = null;
                for (int i = 0, j = transPropertys.Length; i < j; i++)
                {
                    pInfo = transPropertys[i];
                    if (skipPropertyFn != null && skipPropertyFn(pInfo)) continue;

                    currentConfig = transConfig[i];
                    currentBytes = SpecUtil.GetGetHostBytes(pInfo.PropertyType, pInfo.GetValue(this, null));
                    if (currentBytes != null && currentBytes.Length > 0)
                    {
                        if (currentConfig.Reverse) currentBytes = SpecUtil.ReverseBytes(currentBytes);
                        ms.Write(currentBytes, 0, currentBytes.Length);

                        //Console.WriteLine("【{3}】获取属性[{0}]的传输字节序列，长度: {1}，反转序列：{2}, 数据类型:{4}",
                        //    pInfo.Name, currentBytes.Length,
                        //    currentConfig.Reverse,
                        //    this.GetType().FullName,
                        //    pInfo.PropertyType);
                    }

                }
                return ms.ToArray();
            }
        }

        /// <summary>
        /// 从网络字节序列中加载数据
        /// </summary>
        /// <param name="networkBytes"></param>
        public virtual void LoadFromNetworkBytes(byte[] networkBytes)
        {
            MemoryStream ms = new MemoryStream(networkBytes);
            SpecUtil.BindFromNetworkStream(this, ms, 0, false);
            ms.Dispose();
        }

        #endregion

        /// <summary>
        /// 判断当前实例类型绑定的相关网络序列是否有效
        /// </summary>
        /// <typeparam name="TEntity">实例类型</typeparam>
        /// <param name="ntkBytes">网络序列</param>
        /// <param name="diffWriter">二进制标记文本输出</param>
        /// <returns>如果是有效的的协议数据则返回为true。</returns>
        public static ValidESPDataBaseWrap IsValidESPInstance<TEntity>(byte[] ntkBytes, TextWriter diffWriter)
           where TEntity : ESPDataBase, new()
        {
            ValidESPDataBaseWrap wrap = new ValidESPDataBaseWrap();
            ESPDataBase instance = null;
            try
            {
                wrap.IsValid = true;
                instance = ESPDataBase.BindFromNetworkBytes<TEntity>(ntkBytes);
            }
            catch (Exception bindExp)
            {
                wrap.IsValid = false;
                instance = new TEntity();
                if (diffWriter != null)
                {
                    string outMsg = string.Empty;
                    bindExp = SpecUtil.GetTriggerException(bindExp, ref outMsg);
                    diffWriter.WriteLine("*对象绑定失败，异常：{0}", outMsg + bindExp.ToString());
                }
            }

            byte[] bytes2cmp = instance.GetNetworkBytes();
            bool blnResult = SpecUtil.AreEqual(bytes2cmp, ntkBytes);
            if (diffWriter != null && blnResult == false)
            {
                diffWriter.WriteLine("数据类型：{0}", typeof(TEntity).AssemblyQualifiedName);
                diffWriter.WriteLine("原始序列 长度:{0}\r\n{1}", ntkBytes.Length, ntkBytes.GetHexViewString());
                diffWriter.WriteLine();
                diffWriter.WriteLine("绑定序列 长度:{0}\r\n{1}", bytes2cmp.Length, bytes2cmp.GetHexViewString());
            }
            wrap.Instance = instance;
            return wrap;
        }

        /// <summary>
        /// 判断当前实例类型绑定的相关网络序列是否有效
        /// </summary>
        /// <typeparam name="TEntity">实例类型</typeparam>
        /// <param name="ntkBytes">网络序列</param>
        /// <param name="diffWriter">The diff writer.</param>
        /// <returns>
        /// 	<c>true</c> if [is valid network bytes] [the specified NTK bytes]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValidNetworkBytes<TEntity>(byte[] ntkBytes, TextWriter diffWriter)
           where TEntity : ESPDataBase, new()
        {
            return IsValidESPInstance<TEntity>(ntkBytes, diffWriter).IsValid;
        }

        /// <summary>
        /// 判断是否是自身有效的字节序列
        /// </summary>
        /// <param name="ntkBytes">网络字节序列</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="diffWriter">The diff writer.</param>
        /// <param name="instance">输出有效的实例</param>
        /// <returns>
        /// 	<c>true</c> if [is valid instance] [the specified NTK bytes]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValidInstance(byte[] ntkBytes, Type targetType, TextWriter diffWriter, out ESPDataBase instance)
        {
            MethodInfo gm = typeof(ESPDataBase).GetMethod("IsValidESPInstance",
                BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Static);
            gm = gm.MakeGenericMethod(targetType);

            ValidESPDataBaseWrap wrap = gm.Invoke(null, new object[] { ntkBytes, diffWriter }) as ValidESPDataBaseWrap;
            if (wrap != null)
            {
                instance = wrap.Instance;
                return wrap.IsValid;
            }
            else
            {
                instance = null;
                return false;
            }
        }

        #region IDataIndex 成员

        private long[] _mapRng = new long[] { -1, -1 };
        /// <summary>
        /// 数据存放位置索引的开始与结束位置(长度为2)
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public long[] ContentRange
        {
            get
            {
                return _mapRng;
            }

            set
            {
                if (value.Length > 0 && value.Length == 2)
                {
                    _mapRng = value;
                }
            }
        }

        private bool _justMapped = true;
        /// <summary>
        /// 获取或设置当前对象是否只绑定了索引映射位置,而没有绑定内容数据.(默认为true)
        /// </summary>
        /// <value></value>
        [System.Xml.Serialization.XmlIgnore]
        public bool JustMapped
        {
            get { return _justMapped; }
            set { _justMapped = value; }
        }

        /// <summary>
        /// 获取数据类型存储的顺序
        /// </summary>
        //public abstract DataStruct[] IndexTypes { get; }

        #endregion

        private long _stmPos = 0;

        /// <summary>
        /// 保存当前字节序列流的位置
        /// </summary>
        /// <param name="stm">The STM.</param>
        protected void SetCurrentPosition(Stream stm)
        {
            _stmPos = stm.Position;
        }

        /// <summary>
        /// 还原当前字节序列流的位置
        /// </summary>
        /// <param name="stm">The STM.</param>
        protected void RestorePosition(Stream stm)
        {
            stm.Position = _stmPos;
        }

        private ESPContext _context;
        /// <summary>
        /// 获取或设置当前对象的绑定上下文
        /// </summary>
        public ESPContext Context
        {
            get { return _context; }
            set { _context = value; }
        }

        #region 基类辅助函数
        /// <summary>
        /// 自定义属性绑定词典
        /// </summary>
        public virtual void CustomPropertyBindAction() { }

        #endregion
        /// <summary>
        /// 绑定内置数据
        /// </summary>
        public virtual void BindMappingWithStream(Stream mapStm)
        {
            JustMapped = false;
        }

        /// <summary>
        /// 采用通用方法绑定实例数据
        /// </summary>
        /// <param name="ntkStm">相关映射网络序列</param>
        /// <param name="idxBegin">映射开始索引</param>
        /// <param name="isJustMappingBind">是否只是绑定映射关系</param>
        public virtual void BindFromNetworkStream(Stream ntkStm, long idxBegin, bool isJustMappingBind)
        {
            SpecUtil.BindFromNetworkStream(this, ntkStm, idxBegin, isJustMappingBind);
        }

        /// <summary>
        /// 从网络字节序列绑定实例
        /// </summary>
        /// <param name="ntkBytes">网络字节序列</param>
        public static TEntity BindFromNetworkBytes<TEntity>(byte[] ntkBytes)
            where TEntity : ESPDataBase, new()
        {
            TEntity instance = new TEntity();
            MemoryStream ms = new MemoryStream(ntkBytes);
            SpecUtil.BindFromNetworkStream(instance, ms, 0, false);
            ms.Dispose();
            return instance;
        }

    }

    /// <summary>
    /// 有效ESP数据对象封装
    /// </summary>
    [Serializable]
    public class ValidESPDataBaseWrap
    {
        /// <summary>
        /// 获取或设置该对象是否有效
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// 获取或设置有效的对象实例
        /// </summary>
        public ESPDataBase Instance { get; set; }
    }



}

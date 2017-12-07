using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;

namespace Gwsoft.DataSpec
{
    /// <summary>
    /// 辅助函数类
    /// </summary>
    public static class SpecUtil
    {
        /// <summary>
        /// 反转二进制字节序列
        /// </summary>
        public static byte[] ReverseBytes(this byte[] bytes)
        {
            int num = bytes.Length / 2;
            byte by;
            int idx;
            for (int i = 0; i < num; i++)
            {
                by = bytes[i];
                idx = bytes.Length - i - 1;
                bytes[i] = bytes[idx];
                bytes[idx] = by;
            }
            return bytes;
        }

        /// <summary>
        /// UTF-8编码签名字节序列
        /// </summary>
        public static byte[] UTF8_BOM_BYTES = new byte[] { 0xEF, 0xBB, 0xBF };

        /// <summary>
        /// 删除以前置字节内容的字节序列并返回为新字节序列(如果有)
        /// </summary>
        public static byte[] TrimStart(this byte[] bytes, byte[] startBytes)
        {
            byte[] retBytes = bytes;
            if (bytes != null && bytes.Length > startBytes.Length)
            {
                int j = startBytes.Length;
                if (BytesStartWith(bytes, startBytes, 0))
                {
                    retBytes = new byte[bytes.Length - j];
                    Buffer.BlockCopy(bytes, j, retBytes, 0, bytes.Length - j);
                    bytes = retBytes;
                }
            }
            return retBytes;
        }

        /// <summary>
        /// 判断字节流是否以指定前缀开始
        /// </summary>
        public static bool BytesStartWith(this byte[] srcBytes, byte[] prefixBytes, int startAt)
        {
            if (srcBytes != null && srcBytes.Length - startAt >= prefixBytes.Length)
            {
                bool blnDoTrim = true;
                int j = prefixBytes.Length;
                for (int i = 0; i < j; i++)
                {
                    if (srcBytes[i + startAt] != prefixBytes[i])
                    {
                        blnDoTrim = false;
                        break;
                    }
                }
                return blnDoTrim;
            }
            return false;
        }

        /// <summary>
        /// 判断字节流是否以指定前缀开始
        /// </summary>
        public static bool BytesStartWith(this byte[] srcBytes, byte[] prefixBytes)
        {
            return BytesStartWith(srcBytes, prefixBytes, 0);
        }

        /// <summary>
        /// 添加以前置字节内容的字节序列并返回为新字节序列(如果没有)
        /// </summary>
        public static byte[] AddPrefix(this byte[] bytes, byte[] startBytes)
        {
            byte[] retBytes = bytes;

            if (bytes != null)
            {
                bool blnDoAdd = false;
                int j = startBytes.Length;

                if (bytes.Length < j)
                {
                    blnDoAdd = true;
                }
                else
                {
                    for (int i = 0; i < j; i++)
                    {
                        if (bytes[i] != startBytes[i])
                        {
                            blnDoAdd = true;
                            break;
                        }
                    }
                }

                if (blnDoAdd)
                {
                    retBytes = new byte[bytes.Length + j];
                    Buffer.BlockCopy(bytes, 0, retBytes, j, bytes.Length);

                    for (int m = 0; m < j; m++)
                    {
                        retBytes[m] = startBytes[m];
                    }

                    bytes = retBytes;
                }
            }

            return retBytes;
        }

        /// <summary>
        /// 获取对象序列化的二进制版本
        /// </summary>
        /// <param name="pObj">对象实体</param>
        /// <returns>如果对象实体为Null，则返回结果为Null。</returns>
        public static byte[] GetSerializeBytes(this object pObj)
        {
            if (pObj == null) { return null; }
            MemoryStream serializationStream = new MemoryStream();
            new BinaryFormatter().Serialize(serializationStream, pObj);
            serializationStream.Position = 0L;
            byte[] buffer = new byte[serializationStream.Length];
            serializationStream.Read(buffer, 0, buffer.Length);
            serializationStream.Close();
            return buffer;
        }

        /// <summary>
        /// 从已序列化数据中(byte[])获取对象实体
        /// </summary>
        /// <param name="binData">二进制数据</param>
        /// <returns>对象实体</returns>
        public static object GetSerializedObject(this byte[] binData)
        {
            if (binData == null) return null;
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream serializationStream = new MemoryStream(binData);
            return formatter.Deserialize(serializationStream);
        }

        /// <summary>
        /// 获取二进制存储的字节长度(BitConvert类兼容的字节长度)
        /// <para>返回-1则长度未知</para>
        /// </summary>
        public static long GetCLRTypeByteLength(this Type DataType)
        {
            long retLen = -1;
            TypeCode cTCode = Type.GetTypeCode(DataType);
            switch (cTCode)
            {
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Boolean:
                case TypeCode.Char:
                    retLen = 1L;
                    break;

                case TypeCode.Int16:
                case TypeCode.UInt16:
                    retLen = 2;
                    break;

                case TypeCode.Single:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                    retLen = 4;
                    break;

                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Double:
                    retLen = 8;
                    break;

                case TypeCode.Object:
                    break;

                default:
                    break;
            }
            return retLen;
        }

        /// <summary>
        /// 读取网络IO流中当前位置长度的字节序列
        /// </summary>
        /// <param name="stmRead">目标读取IO流</param>
        /// <param name="tByteReadLen">要读取的总长度</param>
        /// <returns></returns>
        public static byte[] ReadNetworkStreamBytes(this Stream stmRead, int tByteReadLen)
        {
            return ReadNetworkStreamBytes(stmRead, tByteReadLen, false);
        }

        /// <summary>
        /// 读取网络IO流中当前位置长度的字节序列
        /// </summary>
        /// <param name="stmRead">目标读取IO流</param>
        /// <param name="tByteReadLen">要读取的总长度</param>
        /// <param name="autoFixEndRange">是否自动修正末尾字节序列越界</param>
        /// <returns></returns>
        public static byte[] ReadNetworkStreamBytes(this Stream stmRead, int tByteReadLen, bool autoFixEndRange)
        {
            //byte[] result = new byte[tByteReadLen];
            using (MemoryStream ms = new MemoryStream())
            {
                if (stmRead != null && stmRead.CanRead)
                {
                    //FIX:修复越界操作
                    if (autoFixEndRange)
                    {
                        TryDoStreamAction(stmRead, s =>
                        {
                            if (s.Position + tByteReadLen > s.Length && s.Length - tByteReadLen > 0)
                                s.Position = s.Length - tByteReadLen;
                        });
                    }

                    int currentRead = 0, rc = 1;
                    int bufferLen = (tByteReadLen < 2048) ? tByteReadLen : 2048;
                    byte[] buffer = new byte[bufferLen];

                    while (stmRead.CanRead && rc > 0 && currentRead < tByteReadLen)
                    {
                        if (currentRead + buffer.Length > tByteReadLen)
                        {
                            rc = stmRead.Read(buffer, 0, tByteReadLen - currentRead);
                        }
                        else
                        {
                            rc = stmRead.Read(buffer, 0, buffer.Length);
                        }
                        ms.Write(buffer, 0, rc);
                        //System.Diagnostics.Trace.WriteLine(string.Format("CurrentRead:{0}, TotalRead:{1}, TargetLength:{2}", rc, currentRead, result.Length));
                        currentRead += rc;
                    }

                    //if (currentRead < tByteReadLen)
                    //{
                    //    throw new BadSpecDataException("在当前位置，没有指定长度的字节可供读取，已读取[" + currentRead + "/" + tByteReadLen + "]字节！");
                    //}
                }
                return ms.ToArray();
            }
        }

        /// <summary>
        /// 读取网络字节序列到指定数据类型的实例
        /// </summary>
        public static TEntity ReadNetworkStreamAsEntity<TEntity>(this Stream stmRead)
        {
            return ReadNetworkStreamAsEntity<TEntity>(stmRead, null);
        }

        /// <summary>
        /// 读取网络字节序列到指定数据类型的实例(可选指定长度)
        /// </summary>
        /// <typeparam name="TEntity">实例类型</typeparam>
        /// <param name="stmRead">要读取的网络IO流</param>
        /// <param name="tByteReadLen">目标读取的长度（可选），若指定请确保长度正确！</param>
        /// <returns></returns>
        public static TEntity ReadNetworkStreamAsEntity<TEntity>(this Stream stmRead, int? tByteReadLen)
        {
            Type EntityType = typeof(TEntity);
            int readLen = -1;
            if (tByteReadLen != null && tByteReadLen.HasValue)
            {
                readLen = tByteReadLen.Value;
            }
            else
            {
                readLen = (int)GetCLRTypeByteLength(EntityType);
            }

            if (readLen == -1) throw new InvalidOperationException("不可识别数据类型长度，请指定要读取的字节长度(tByteReadLen)！");
            return ReadStreamAsEntity<TEntity>(true, stmRead, readLen, GetNetworkByteParser(EntityType));
        }

        /// <summary>
        /// 在指定IO流中通过解析字节流函数直接解析为相关实例
        /// </summary>
        /// <param name="throwError">是否抛出异常</param>
        /// <param name="stmRead">IO流</param>
        /// <param name="length">读取长度</param>
        /// <param name="networkbytesParseFn">解析函数</param>
        /// <returns>相关实例数据，如果解析错误则返回null。</returns>
        public static object ReadStreamAsObject(bool throwError, Stream stmRead, int length, Func<byte[], object> networkbytesParseFn)
        {
            if (networkbytesParseFn == null) throw new InvalidOperationException("解析函数不能为null!");
            object result = null;
            if (stmRead != null && stmRead.CanRead)
            {
                byte[] dat = ReadNetworkStreamBytes(stmRead, length, false);
                try
                {
                    result = networkbytesParseFn(dat);
                }
                catch
                {
                    if (throwError) throw;
                }
            }
            return result;
        }

        /// <summary>
        /// 在指定IO流中通过解析字节流函数直接解析为相关实例
        /// </summary>
        /// <typeparam name="TEntity">实例类型</typeparam>
        /// <param name="throwError">是否抛出异常</param>
        /// <param name="stmRead">IO流</param>
        /// <param name="length">读取长度</param>
        /// <param name="networkbytesParseFn">解析函数</param>
        /// <returns>相关实例类型数据，如果解析错误则返回默认实例类型的值。</returns>
        public static TEntity ReadStreamAsEntity<TEntity>(bool throwError, Stream stmRead, int length, Func<byte[], object> networkbytesParseFn)
        {
            if (networkbytesParseFn == null) throw new InvalidOperationException("解析函数不能为null!");

            TEntity result = default(TEntity);
            if (stmRead != null && stmRead.CanRead)
            {
                byte[] dat = ReadNetworkStreamBytes(stmRead, length, false);
                try
                {
                    result = (TEntity)networkbytesParseFn(dat);
                }
                catch
                {
                    if (throwError) throw;
                }
            }
            return result;
        }

        /// <summary>
        /// 获取该数据类型的主机字节序列
        /// </summary>
        public static byte[] GetGetHostBytes(Type DataType, object instance)
        {
            if (instance == null) return SpecUtil.EmptyBytes;
            byte[] retBytes = new byte[0];
            TypeCode cTCode = Type.GetTypeCode(DataType);
            //Console.WriteLine("TypeCode:{0}, Type:{1}", cTCode, DataType);
            switch (cTCode)
            {
                #region 类型码依次转换

                #region 基本类型
                case TypeCode.SByte:
                    retBytes = new byte[] { Convert.ToByte(Convert.ToSByte(instance)) };
                    break;

                case TypeCode.Byte:
                    retBytes = new byte[] { Convert.ToByte(instance) };
                    break;

                case TypeCode.Boolean:
                    retBytes = BitConverter.GetBytes(Convert.ToBoolean(instance));
                    break;

                case TypeCode.Char:
                    retBytes = BitConverter.GetBytes(Convert.ToChar(instance));
                    break;

                case TypeCode.Double:
                    retBytes = BitConverter.GetBytes(Convert.ToDouble(instance));
                    break;

                case TypeCode.Single:
                    retBytes = BitConverter.GetBytes(Convert.ToSingle(instance));
                    break;

                case TypeCode.Int16:
                    retBytes = BitConverter.GetBytes(Convert.ToInt16(instance));
                    break;

                case TypeCode.Int32:
                    retBytes = BitConverter.GetBytes(Convert.ToInt32(instance));
                    break;

                case TypeCode.Int64:
                    retBytes = BitConverter.GetBytes(Convert.ToInt64(instance));
                    break;

                case TypeCode.UInt16:
                    retBytes = BitConverter.GetBytes(Convert.ToUInt16(instance));
                    break;

                case TypeCode.UInt32:
                    retBytes = BitConverter.GetBytes(Convert.ToUInt32(instance));
                    break;

                case TypeCode.UInt64:
                    retBytes = BitConverter.GetBytes(Convert.ToUInt64(instance));
                    break;
                #endregion

                case TypeCode.Object:
                    if (DataType.GetInterface(typeof(IByteTransfer).FullName) != null)
                    {
                        //Console.WriteLine("???? TypeCode:{0}, Type:{1}", cTCode, DataType);
                        retBytes = ((IByteTransfer)instance).GetHostBytes();
                    }
                    else if (DataType.IsArray)
                    {
                        Type ElementType = DataType.GetElementType();
                        if (ElementType == typeof(byte))
                        {
                            //字节序列则直接返回
                            //Console.WriteLine("!!! TypeCode:{0}, Type:{1}", cTCode, DataType);
                            retBytes = (byte[])instance;
                            //Console.WriteLine("返回字节长度 => {0}", retBytes.Length);
                        }
                        else
                        {
                            //Console.WriteLine("install is null = {0}", instance == null);
                            if (ElementType.GetInterface(typeof(IByteTransfer).FullName) != null)
                            {
                                IByteTransfer[] items = (IByteTransfer[])instance;

                                if (items == null)
                                {
                                    throw new InvalidOperationException(string.Concat("类型：", DataType.GetElementType(), ",转换失败！"));
                                }
                                else
                                {
                                    MemoryStream ms = new MemoryStream();
                                    byte[] itemBytes = new byte[0];
                                    for (int i = 0, j = items.Length; i < j; i++)
                                    {
                                        itemBytes = items[i].GetHostBytes();
                                        ms.Write(itemBytes, 0, itemBytes.Length);
                                    }
                                    retBytes = ms.ToArray();
                                    ms.Dispose();
                                }
                            }
                            else
                            {
                                throw new InvalidOperationException(string.Concat("类型：", DataType.GetElementType(), ",暂不支持协议传输！"));
                            }
                        }
                    }
                    else if (DataType.IsSerializable && instance != null)
                    {
                        //Console.WriteLine("TypeCode:{0}, Type:{1}", cTCode, DataType);
                        throw new InvalidOperationException(string.Concat("类型：", DataType, ",暂不支持原生对象的二进制序列化！"));
                        //retBytes = GetSerializeBytes(instance);
                    }
                    break;

                default:
                    break;
                #endregion
            }
            return retBytes;
        }

        private static byte[] _emptyBytes = new byte[0];
        /// <summary>
        /// 空字节序列实例
        /// </summary>
        public static byte[] EmptyBytes
        {
            get { return _emptyBytes; }
        }

        /// <summary>
        /// 基本（普通）数据类型的传输类型码(2010-1-4 by ridge)
        /// </summary>
        public static TypeCode[] SpecSupportTransferTypeCode = new TypeCode[] { 
           TypeCode.SByte, TypeCode.Byte, TypeCode.Boolean, TypeCode.Char,
           TypeCode.Int16, TypeCode.UInt16,
           TypeCode.Single, TypeCode.Int32, TypeCode.UInt32,
           TypeCode.Int64, TypeCode.UInt64, TypeCode.Double
        };

        internal static object cacheLock = new object();
        /// <summary>
        /// 对象属性存储顺序缓存
        /// </summary>
        internal static ThreadSafeDictionary<Type, PropertyInfo[]> NetworkByteBindDict = new ThreadSafeDictionary<Type, PropertyInfo[]>();

        /// <summary>
        /// 对象属性传输顺序缓存
        /// </summary>
        internal static ThreadSafeDictionary<Type, ObjectTransferOrderAttribute[]> TypeTransConfigDict = new ThreadSafeDictionary<Type, ObjectTransferOrderAttribute[]>();

        /// <summary>
        /// 获取指定对象类型的属性上配置为传输的顺序属性数组
        /// <para>在公开属性在配置数据传输属性:ObjectTransferOrderAttribute</para>
        /// </summary>
        /// <param name="instanceType">配置实例类型</param>
        /// <param name="transConfig">相对应的顺序配置信息</param>
        /// <returns></returns>
        public static PropertyInfo[] GetTransferProperties(Type instanceType, out ObjectTransferOrderAttribute[] transConfig)
        {
            if (!NetworkByteBindDict.ContainsKey(instanceType))
            {
                Type sortAttrType = typeof(ObjectTransferOrderAttribute);

                SortedList<float, PropertyInfo> bndProps = new SortedList<float, PropertyInfo>();
                SortedDictionary<float, ObjectTransferOrderAttribute> configs = new SortedDictionary<float, ObjectTransferOrderAttribute>();
                lock (cacheLock)
                {
                    PropertyInfo[] pubProperties = instanceType.GetProperties();
                    for (int m = 0, n = pubProperties.Length; m < n; m++)
                    {
                        if (Attribute.IsDefined(pubProperties[m], sortAttrType, false))
                        {
                            ObjectTransferOrderAttribute sort = Attribute.GetCustomAttribute(pubProperties[m], sortAttrType, false) as ObjectTransferOrderAttribute;
                            float key = (float)sort.Order + sort.SubOrder;
                            bndProps.Add(key, pubProperties[m]);
                            configs.Add(key, sort);
                        }
                    }
                }

                ObjectTransferOrderAttribute[] nAttrs = new ObjectTransferOrderAttribute[configs.Values.Count];
                configs.Values.CopyTo(nAttrs, 0);

                //有排序顺序的属性列表
                PropertyInfo[] nProps = new PropertyInfo[bndProps.Values.Count];
                bndProps.Values.CopyTo(nProps, 0);

                #region 2010-8-18 同步加锁
                lock (TypeTransConfigDict)
                {
                    if (TypeTransConfigDict.ContainsKey(instanceType))
                    {
                        TypeTransConfigDict[instanceType] = nAttrs;
                    }
                    else
                    {
                        TypeTransConfigDict.Add(instanceType, nAttrs);
                    }
                }

                lock (NetworkByteBindDict)
                {
                    if (NetworkByteBindDict.ContainsKey(instanceType))
                    {
                        NetworkByteBindDict[instanceType] = nProps;
                    }
                    else
                    {
                        NetworkByteBindDict.Add(instanceType, nProps);
                    }
                }
                #endregion
            }

            transConfig = TypeTransConfigDict[instanceType];
            return NetworkByteBindDict[instanceType];
        }

        /// <summary>
        /// 获取指定对象类型的属性上配置为传输的顺序属性数组
        /// <para>在公开属性在配置数据传输属性:ObjectTransferOrderAttribute</para>
        /// </summary>
        public static PropertyInfo[] GetTransferProperties(Type instanceType)
        {
            ObjectTransferOrderAttribute[] configs = new ObjectTransferOrderAttribute[0];
            return GetTransferProperties(instanceType, out configs);
        }

        /// <summary>
        /// 获取堆栈中最底层触发的异常
        /// </summary>
        /// <param name="exp">当前异常</param>
        /// <returns></returns>
        public static Exception GetTriggerException(this Exception exp)
        {
            while (exp.InnerException != null)
            {
                exp = exp.InnerException;
            }
            return exp;
        }

        /// <summary>
        /// 获取堆栈中最底层触发的异常
        /// </summary>
        /// <param name="exp">当前异常</param>
        /// <param name="expSrcMsg">异常消息引用</param>
        /// <returns></returns>
        public static Exception GetTriggerException(this Exception exp, ref string expSrcMsg)
        {
            while (exp.InnerException != null)
            {
                expSrcMsg += string.Format("\r\n******************************\r\nType:{0}\r\nMessage:{2}\r\nStackTrace:\r\n{1}\r\n******************************\r\n",
                    exp.GetType().FullName, exp.StackTrace, exp.Message);
                exp = exp.InnerException;
            }
            return exp;
        }

        /// <summary>
        /// 从二进制序列读取数据
        /// </summary>
        /// <param name="codeData">二进制序列</param>
        /// <param name="reverseBytes">是否反转字节序列</param>
        /// <param name="rdrFun">读取解析函数</param>
        /// <returns></returns>
        public static object ObjectFromBytes(byte[] codeData, bool reverseBytes, Func<BinaryReader, object> rdrFun)
        {
            object objRet = null;
            if (reverseBytes) codeData = ReverseBytes(codeData);
            MemoryStream ms = new MemoryStream(codeData);
            ms.Position = 0;

            BinaryReader rdr = new BinaryReader(ms);
            objRet = rdrFun(rdr);
            rdr.Close();
            ms.Dispose();
            return objRet;
        }

        /// <summary>
        /// 获取指定数据类型在字节流上的映射数据
        /// </summary>
        /// <param name="dataType">数据类型.</param>
        /// <param name="context">The context.</param>
        /// <param name="ntkStm">网络字节序</param>
        /// <param name="posAt">The pos at.</param>
        /// <param name="iFirstOrder">第一个绑定的索引排序</param>
        /// <param name="objByteLen">输出当前对象所占用的字节长度</param>
        /// <returns></returns>
        public static object ObjectFromStream(this Type dataType, ESPContext context, Stream ntkStm, long posAt, int iFirstOrder, out long objByteLen)
        {
            object objRet = null;
            objByteLen = 0L;

            if (!dataType.IsSubclassOf(typeof(ESPDataBase)))
            {
                objRet = CLRObjectFromStream(dataType, ntkStm);
                objByteLen = GetCLRTypeByteLength(dataType);
            }
            else
            {
                objRet = Activator.CreateInstance(dataType, context);

                if (((ESPDataBase)objRet).HasImplementDataBind)
                {
                    ((ESPDataBase)objRet).BindMappingWithStream(ntkStm);
                }
                else
                {
                    BindFromNetworkStream((ESPDataBase)objRet, ntkStm, posAt, false, iFirstOrder);
                }

                objByteLen = ((ESPDataBase)objRet).GetContentLength();
            }
            return objRet;
        }

        internal static object CLRObjectFromStream(this Type DataType, Stream ntkStm)
        {
            object objRet = null;

            BinaryReader reader = new BinaryReader(ntkStm);
            TypeCode cTCode = Type.GetTypeCode(DataType);

            switch (cTCode)
            {
                #region 普通类型CLR
                case TypeCode.SByte:
                    objRet = reader.ReadSByte();
                    break;

                case TypeCode.Byte:
                    objRet = reader.ReadByte();
                    break;

                case TypeCode.Boolean:
                    objRet = reader.ReadBoolean();
                    break;

                case TypeCode.Char:
                    objRet = reader.ReadChar();
                    break;

                case TypeCode.Double:
                    objRet = ObjectFromBytes(reader.ReadBytes(8), true, (rdr) => rdr.ReadDouble());
                    break;

                case TypeCode.Single:
                    objRet = ObjectFromBytes(reader.ReadBytes(4), true, (rdr) => rdr.ReadSingle());
                    break;

                case TypeCode.Int16:
                    objRet = ObjectFromBytes(reader.ReadBytes(2), true, (rdr) => rdr.ReadInt16());
                    break;

                case TypeCode.Int32:
                    objRet = ObjectFromBytes(reader.ReadBytes(4), true, (rdr) => rdr.ReadInt32());
                    break;

                case TypeCode.Int64:
                    objRet = ObjectFromBytes(reader.ReadBytes(8), true, (rdr) => rdr.ReadInt64());
                    break;

                case TypeCode.UInt16:
                    objRet = ObjectFromBytes(reader.ReadBytes(2), true, (rdr) => rdr.ReadUInt16());
                    break;

                case TypeCode.UInt32:
                    objRet = ObjectFromBytes(reader.ReadBytes(4), true, (rdr) => rdr.ReadUInt32());
                    break;

                case TypeCode.UInt64:
                    objRet = ObjectFromBytes(reader.ReadBytes(8), true, (rdr) => rdr.ReadUInt64());
                    break;

                #endregion

                case TypeCode.Object:
                    throw new InvalidOperationException("类型" + DataType + "暂不支持!");
                default:
                    break;
            }

            return objRet;
        }

        /// <summary>
        /// 获取普通对象的字节长度计算
        /// </summary>
        /// <param name="objDataType">数据类型</param>
        /// <param name="objInstance">实例</param>
        /// <returns></returns>
        internal static long GetObjectByteLength(Type objDataType, object objInstance)
        {
            TypeCode lastTypeCode = Type.GetTypeCode(objDataType);
            if (Array.IndexOf<TypeCode>(SpecSupportTransferTypeCode, lastTypeCode) != -1)
            {
                return GetCLRTypeByteLength(objDataType);
            }
            else
            {
                if (!objDataType.IsSubclassOf(typeof(ESPDataBase)))
                {
                    throw new InvalidOperationException("数组属性的单元项(" + objDataType.FullName + ")数据类型必须是" + typeof(ESPDataBase).FullName + "继承类型!");
                }
                else
                {
                    return ((ESPDataBase)objInstance).GetContentLength();
                }
            }
        }

        /// <summary>
        /// 获取数组数据的字节长度计算
        /// </summary>
        /// <param name="elementType">数组成员数据类型</param>
        /// <param name="arrLength">数组长度</param>
        /// <param name="arrInstance">当前数组实例</param>
        /// <returns></returns>
        internal static long GetArrayByteLength(Type elementType, int arrLength, object arrInstance)
        {
            TypeCode lastTypeCode = Type.GetTypeCode(elementType);
            long subItemOffset = 0;

            long lastOffset = -1;
            if (Array.IndexOf<TypeCode>(SpecSupportTransferTypeCode, lastTypeCode) != -1)
            {
                subItemOffset = GetCLRTypeByteLength(elementType);
                if (subItemOffset == -1)
                {
                    //错误的数据类型
                    throw new InvalidOperationException("数组属性的单元项(" + elementType.FullName + ")数据类型不被支持!");
                }
                else
                {
                    lastOffset = subItemOffset * Convert.ToInt64(arrLength);
                }
            }
            else
            {
                if (!elementType.IsSubclassOf(typeof(ESPDataBase)))
                {
                    throw new InvalidOperationException("数组属性的单元项(" + elementType.FullName + ")数据类型必须是" + typeof(ESPDataBase).FullName + "继承类型!");
                }
                else
                {
                    ESPDataBase[] subItems = (ESPDataBase[])arrInstance;
                    if (subItems != null)
                    {
                        for (int p = 0, q = subItems.Length; p < q; p++)
                        {
                            subItemOffset += subItems[p].GetContentLength();
                        }
                    }
                    lastOffset = subItemOffset;
                }
            }

            return lastOffset;
        }

        /// <summary>
        /// 获取当前IO流中指定对象类型的长度数组数据
        /// </summary>
        /// <param name="ntkStm">绑定映射IO流</param>
        /// <param name="idxBegin">字节序列游标计数</param>
        /// <param name="elementType">数组内数据类型</param>
        /// <param name="arrLength">数组长度</param>
        /// <param name="arrayByteLen">当前数组所占用的字节长度</param>
        /// <returns></returns>
        internal static object ArrayObjectFromStream(Stream ntkStm, long idxBegin, Type elementType, int arrLength, out long arrayByteLen)
        {
            arrayByteLen = arrLength;
            TypeCode lastTypeCode = Type.GetTypeCode(elementType);
            if (lastTypeCode == TypeCode.Byte)
            {
                return ReadNetworkStreamBytes(ntkStm, arrLength);
            }

            Array arr = Array.CreateInstance(elementType, arrLength);
            if (Array.IndexOf<TypeCode>(SpecSupportTransferTypeCode, lastTypeCode) != -1)
            {
                int elByteLen = (int)GetCLRTypeByteLength(elementType);
                Func<byte[], object> objParser = GetNetworkByteParser(elementType);
                for (int i = 0, j = arr.Length; i < j; i++)
                {
                    arr.SetValue(ReadStreamAsObject(true, ntkStm, elByteLen, objParser), i);
                }

                //定长元素计算
                arrayByteLen = elByteLen * arrLength;
            }
            else
            {
                if (!elementType.IsSubclassOf(typeof(ESPDataBase)))
                {
                    throw new InvalidOperationException("数组属性的单元项(" + elementType.FullName + ")数据类型必须是" + typeof(ESPDataBase).FullName + "继承类型!");
                }
                else
                {
                    arrayByteLen = 0L;
                    for (int m = 0, n = arr.Length; m < n; m++)
                    {
                        ESPDataBase instance = Activator.CreateInstance(elementType) as ESPDataBase;
                        if (instance != null)
                        {
                            try
                            {
                                BindFromNetworkStream(instance, ntkStm, idxBegin + arrayByteLen, false);
                                arrayByteLen += instance.GetContentLength();
                            }
                            catch (Exception exp)
                            {
                                throw new InvalidDataException(string.Format("*获取数组元素[{0}/{1}]{5}的值失败\r\nType:{2}\r\nMessage:{3}\r\n{4}",
                                    m, n,
                                    exp.GetType().FullName,
                                    exp.Message, exp.StackTrace,
                                    elementType.FullName));
                            }
                        }
                        arr.SetValue(instance, m);
                    }
                }
            }
            return arr;
        }


        /// <summary>
        /// 尝试对序列视图执行的函数操作
        /// </summary>
        /// <param name="streamTarget">序列视图实例</param>
        /// <param name="streamAct">相关执行函数委托</param>
        /// <returns></returns>
        public static bool TryDoStreamAction(Stream streamTarget, Action<Stream> streamAct)
        {
            try
            {
                streamAct(streamTarget);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 尝试对序列视图执行的函数操作
        /// </summary>
        /// <typeparam name="TResult">函数操作结果类型</typeparam>
        /// <param name="streamTarget">序列视图实例</param>
        /// <param name="streamFunc">相关执行函数委托</param>
        /// <param name="errResult">出现异常返回的结果</param>
        /// <returns></returns>
        public static TResult TryGetStreamFunc<TResult>(Stream streamTarget, Func<Stream, TResult> streamFunc, TResult errResult)
        {
            try
            {
                return streamFunc(streamTarget);
            }
            catch (Exception)
            {
                return errResult;
            }
        }


        /// <summary>
        /// 从网络字节序列绑定.net实例数据
        /// </summary>
        /// <param name="instance">待绑定的实例</param>
        /// <param name="ntkStm">网络字节序列</param>
        /// <param name="idxBegin">开始位置, -1表示当前位置。</param>
        /// <param name="isJustMappingBind">是否只是映射绑定,而不绑定数据</param>
        /// <returns>序列当前游标位置</returns>
        public static long BindFromNetworkStream(this ESPDataBase instance, Stream ntkStm, long idxBegin, bool isJustMappingBind)
        {
            return BindFromNetworkStream(instance, ntkStm, idxBegin, isJustMappingBind, 0);
        }

        /// <summary>
        /// 从网络字节序列绑定.net实例数据
        /// </summary>
        /// <param name="instance">待绑定的实例</param>
        /// <param name="ntkStm">网络字节序列</param>
        /// <param name="idxBegin">开始位置, -1表示当前位置。</param>
        /// <param name="isJustMappingBind">是否只是映射绑定,而不绑定数据</param>
        /// <param name="iFirstOrder">第一个属性的排序（默认为0）</param>
        /// <returns>序列当前游标位置</returns>
        public static long BindFromNetworkStream(this ESPDataBase instance, Stream ntkStm, long idxBegin, bool isJustMappingBind, int iFirstOrder)
        {
            long posCursor = idxBegin;
            if (posCursor == -1) posCursor = 0;

            if (iFirstOrder == 0 && instance.HasImplementDataBind)
            {
                try
                {
                    instance.BindMappingWithStream(ntkStm);
                    posCursor = idxBegin + instance.GetContentLength();
                }
                catch (Exception selfBindExp)
                {
                    throw new BadSpecDataException(string.Format("在对象绑定[自实现]{0}时出现错误，当前网络字节位置{1}！",
                        instance.GetType(), TryGetStreamFunc<long>(ntkStm, stm => stm.Position, posCursor), selfBindExp));
                }
                return posCursor - 1;
            }

            //设置区间数据开始索引
            instance.ContentRange[0] = posCursor;
            //Console.WriteLine("====> 字节偏移量:{0}", idxBegin);

            Type instanceType = instance.GetType();
            ObjectTransferOrderAttribute[] transConfigs = new ObjectTransferOrderAttribute[0];
            PropertyInfo[] transProperties = GetTransferProperties(instanceType, out transConfigs);

            object currentObj = null, lastDat = null;

            for (int m = 0, n = transProperties.Length; m < n; m++)
            {
                if (transConfigs[m].Order < iFirstOrder) continue;
                try
                {
                    #region 属性值绑定
                    //Console.WriteLine("绑定属性：{3}::{0}，类型：{1}，偏移量：{2}", transProperties[m].Name, transProperties[m].PropertyType, posCursor, instanceType);
                    long propertyByteLen = 0L;
                    string propKey = BindBuilder.GetPropertyBindKey(instanceType, transProperties[m]);
                    if (BindBuilder.CustomBindDict.ContainsKey(propKey))
                    {
                        #region 自定义属性绑定回调
                        PropertyBindState state = BindBuilder.CustomBindDict[propKey](ntkStm, posCursor, instance);
                        currentObj = state.PropertyValue;
                        if (!state.StreamBind)
                        {
                            propertyByteLen = 0;
                        }
                        else
                        {
                            if (transProperties[m].PropertyType.IsArray)
                            {
                                int arrLen = (int)transProperties[m].PropertyType.GetProperty("Length").GetValue(currentObj, null);
                                propertyByteLen = GetArrayByteLength(transProperties[m].PropertyType.GetElementType(), arrLen, currentObj);
                            }
                            else
                            {
                                propertyByteLen = GetObjectByteLength(transProperties[m].PropertyType, currentObj);
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        #region 通用数据绑定
                        if (!transProperties[m].PropertyType.IsArray)
                        {
                            currentObj = ObjectFromStream(transProperties[m].PropertyType, instance.Context, ntkStm, posCursor, 0, out propertyByteLen);
                        }
                        else
                        {
                            int len = 0;
                            if (transConfigs[m].ArrayLengthOffset == -1)
                            {
                                len = Convert.ToInt32(lastDat);
                            }
                            else
                            {
                                len = Convert.ToInt32(transProperties[m + transConfigs[m].ArrayLengthOffset].GetValue(instance, null));
                            }
                            currentObj = ArrayObjectFromStream(ntkStm, posCursor, transProperties[m].PropertyType.GetElementType(), len, out propertyByteLen);
                        }
                        #endregion
                    }

                    posCursor = posCursor + propertyByteLen;
                    //Console.WriteLine("=>绑定属性:{2}, posCursor:{0}, Len:{1}, Value:{3}", posCursor, propertyByteLen, transProperties[m].Name, currentObj);
                    TryDoStreamAction(ntkStm, s => s.Position = posCursor);
                    #endregion
                }
                catch (Exception bindExp)
                {
                    throw new BadSpecDataException(string.Format("在绑定对象属性{1}::{0}时出现错误，当前网络字节位置{2}！",
                        transProperties[m].Name, transProperties[m].PropertyType, TryGetStreamFunc<long>(ntkStm, s => s.Position, posCursor))
                        , bindExp);
                }
                lastDat = currentObj;
                if (!transConfigs[m].Conditional) transProperties[m].SetValue(instance, currentObj, null);
            }
            //设置区间数据结尾索引
            instance.ContentRange[1] = posCursor - 1;
            return posCursor;

        }

        /// <summary>
        /// 在流当前位置读取为ESPDataBase的实例对象
        /// </summary>
        /// <typeparam name="TEntity">基于<c>ESPDataBase</c>的数据类型</typeparam>
        /// <param name="ntkStm">网络传输IO流</param>
        /// <param name="posCursor">字节序列游标计数</param>
        /// <returns></returns>
        public static TEntity DataBind<TEntity>(this Stream ntkStm, long posCursor)
            where TEntity : ESPDataBase, new()
        {
            TEntity instance = new TEntity();
            instance.BindFromNetworkStream(ntkStm, posCursor, false);
            return instance;
        }

        /// <summary>
        /// 转换为内容流对象
        /// </summary>
        public static MemoryStream AsMemoryStream(this byte[] binDate)
        {
            return new MemoryStream(binDate);
        }

        /// <summary>
        /// 获取容器实例内数组属性的数据绑定
        /// </summary>
        /// <typeparam name="TEntity">数组属性子项的类型</typeparam>
        /// <typeparam name="TEntityContainer">实例容器本身类型约束</typeparam>
        /// <param name="stmBind">当前绑定IO序列流</param>
        /// <param name="container">实例容器本身</param>
        /// <param name="entityLenFetch">数组长度的获取函数委托</param>
        public static TEntity[] GetCurrentContainerEntities<TEntity, TEntityContainer>(this Stream stmBind, TEntityContainer container,
            Func<TEntityContainer, int> entityLenFetch)
            where TEntity : ESPDataBase, new()
        {
            if (container == null || entityLenFetch == null) throw new InvalidOperationException("容器及子项数据绑定对象不能为null！");

            List<TEntity> eList = new List<TEntity>();
            int total = entityLenFetch(container);
            TEntity cEntity = null;
            for (int i = 0, j = total; i < j; i++)
            {
                cEntity = new TEntity();
                BindFromNetworkStream(cEntity, stmBind, stmBind.Position, false);
                eList.Add(cEntity);
            }
            return eList.ToArray();
        }

        /// <summary>
        /// 判断两个数组内数据是否相等
        /// </summary>
        public static bool AreEqual(byte[] srcBytes, byte[] cmpBytes)
        {
            if (srcBytes == null && cmpBytes == null) { return true; }
            if (srcBytes == null || cmpBytes == null) { return false; }
            if (srcBytes.Length != cmpBytes.Length) { return false; }
            for (int i = 0; i < srcBytes.Length; i++)
            {
                if (srcBytes[i] != cmpBytes[i]) { return false; }
            }
            return true;
        }

        /// <summary>
        /// 获取二进制的十六进制查看方式数据
        /// </summary>
        /// <param name="binDat">二进制数据</param>
        /// <returns>二进制的16进制字符形式</returns>
        public static string GetHexViewString(this byte[] binDat)
        {
            byte[] ascByte = new byte[16];
            int lastRead = 0;

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0, j = binDat.Length; i < j; i++)
            {
                if (i == 0)
                {
                    sb.Append("00000000  ");
                }

                sb.Append(binDat[i].ToString("X2") + " ");
                lastRead = i % 16;
                ascByte[lastRead] = binDat[i];

                if (i > 0 && (i + 1) % 8 == 0 && (i + 1) % 16 != 0)
                {
                    sb.Append(" ");
                }

                if (i > 0 && (i + 1) % 16 == 0)
                {
                    sb.Append(" ");
                    foreach (byte chrB in ascByte)
                    {
                        if (chrB >= 0x20 && chrB <= 0x7E) //[32,126]
                        {
                            sb.Append((char)chrB);
                        }
                        else
                        {
                            sb.Append('.');
                        }
                    }

                    if (i + 1 != j)
                    {
                        sb.Append(Environment.NewLine);
                        sb.Append((i + 1).ToString("X2").PadLeft(8, '0') + "  ");
                    }
                }
            }

            if (lastRead < 15)
            {
                sb.Append(new string(' ', (15 - lastRead) * 3));
                if (lastRead < 8) sb.Append(" ");
                sb.Append(" ");
                for (int m = 0; m <= lastRead; m++)
                {
                    byte charL = ascByte[m];
                    if (charL >= 0x20 && charL <= 0x7E) //[32,126]
                    {
                        sb.Append((char)charL);
                    }
                    else
                    {
                        sb.Append('.');
                    }
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 二进制序列的16进制视图形式（16字节换行）
        /// </summary>
        public static string ByteArrayToHexString(this byte[] tBinBytes)
        {
            string draftStr = Regex.Replace(BitConverter.ToString(tBinBytes),
            "([A-z0-9]{2}\\-){16}",
            m =>
            {
                return m.Value.Replace("-", " ") + Environment.NewLine;
            });
            return draftStr.Replace("-", " ");
        }

        /// <summary>
        /// 从原始16进制字符还原到字节序列
        /// </summary>
        public static byte[] HexPatternStringToByteArray(this string hexrawStr)
        {
            if (string.IsNullOrEmpty(hexrawStr))
            {
                return new byte[0];
            }

            string trueRaw = hexrawStr.Replace(" ", "")
                .Replace("\r", "")
                .Replace("\n", "")
                .Replace("-", "")
                .Replace("\t", "").Trim();

            int totalLen = trueRaw.Length;
            if (totalLen % 2 != 0)
            {
                throw new InvalidCastException("hex string size invalid.");
            }
            else
            {
                byte[] rawBin = new byte[totalLen / 2];
                for (int i = 0; i < totalLen; i = i + 2)
                {
                    rawBin[i / 2] = Convert.ToByte(int.Parse(trueRaw.Substring(i, 2),
                        NumberStyles.AllowHexSpecifier));
                }
                return rawBin;
            }
        }

        private static ThreadSafeDictionary<Type, Func<byte[], object>> InternalParseDict = new ThreadSafeDictionary<Type, Func<byte[], object>>();

        /// <summary>
        /// 获取网络字节序列解析函数委托
        /// </summary>
        /// <param name="targetType">本地数据类型</param>
        public static Func<byte[], object> GetNetworkByteParser(Type targetType)
        {
            if (!InternalParseDict.ContainsKey(targetType))
            {
                TypeCode cTCode = Type.GetTypeCode(targetType);
                switch (cTCode)
                {
                    #region 数据类型枚举
                    #region 基本类型
                    case TypeCode.SByte:
                        InternalParseDict.Add(targetType, dat =>
                        {
                            return (sbyte)ObjectFromBytes(dat, true,
                             r => r.ReadSByte());
                        });
                        break;

                    case TypeCode.Byte:
                        InternalParseDict.Add(targetType, dat =>
                        {
                            return (byte)ObjectFromBytes(dat, true,
                             r => r.ReadByte());
                        });
                        break;

                    case TypeCode.Boolean:
                        InternalParseDict.Add(targetType, dat =>
                        {
                            return (bool)ObjectFromBytes(dat, true,
                             r => r.ReadBoolean());
                        });
                        break;

                    case TypeCode.Char:
                        InternalParseDict.Add(targetType, dat =>
                        {
                            return (char)ObjectFromBytes(dat, true,
                             r => r.ReadChar());
                        });
                        break;

                    case TypeCode.Double:
                        InternalParseDict.Add(targetType, dat =>
                        {
                            return (double)ObjectFromBytes(dat, true,
                             r => r.ReadDouble());
                        });
                        break;

                    case TypeCode.Single:
                        InternalParseDict.Add(targetType, dat =>
                        {
                            return (Single)ObjectFromBytes(dat, true,
                             r => r.ReadSingle());
                        });
                        break;

                    case TypeCode.Int16:
                        InternalParseDict.Add(targetType, dat =>
                        {
                            return (short)ObjectFromBytes(dat, true,
                             r => r.ReadInt16());
                        });
                        break;

                    case TypeCode.Int32:
                        InternalParseDict.Add(targetType, dat =>
                        {
                            return (Int32)ObjectFromBytes(dat, true,
                             r => r.ReadInt32());
                        });
                        break;

                    case TypeCode.Int64:
                        InternalParseDict.Add(targetType, dat =>
                        {
                            return (Int64)ObjectFromBytes(dat, true,
                             r => r.ReadInt64());
                        });
                        break;

                    case TypeCode.UInt16:
                        InternalParseDict.Add(targetType, dat =>
                        {
                            return (ushort)ObjectFromBytes(dat, true,
                             r => r.ReadUInt16());
                        });
                        break;

                    case TypeCode.UInt32:
                        InternalParseDict.Add(targetType, dat =>
                        {
                            return (UInt32)ObjectFromBytes(dat, true,
                             r => r.ReadUInt32());
                        });
                        break;

                    case TypeCode.UInt64:
                        InternalParseDict.Add(targetType, dat =>
                        {
                            return (UInt64)ObjectFromBytes(dat, true,
                             r => r.ReadUInt64());
                        });
                        break;
                    #endregion

                    //case TypeCode.Object:
                    //    if (targetType.Equals(typeof(byte[])))
                    //    {
                    //        InternalParseDict.Add(targetType, dat => dat);
                    //    }
                    //    break;  

                    default:
                        InternalParseDict.Add(targetType, dat => dat);
                        break;
                    #endregion
                }
                //Console.WriteLine("Type:{0}, TypeCode:{1}", cTCode, targetType);
            }
            return InternalParseDict[targetType];
        }



    }
}

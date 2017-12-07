using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Gwsoft.DataSpec2
{
    /// <summary>
    /// 以字节序列视图作为数据交互的会话上下文
    /// </summary>
    public class StreamContext : IDisposable
    {
        /// <summary>
        /// 初始化一个 <see cref="StreamContext"/> class 实例。
        /// </summary>
        /// <param name="spec">当前协议规范定义实例</param>
        /// <param name="readStream">读取字节序列</param>
        /// <param name="writeStream">写入字节序列</param>
        public StreamContext(SpecFile spec, Stream readStream, Stream writeStream)
        {
            ContractSpec = spec;
            _reader = new BinaryReader(readStream);
            _writer = new BinaryWriter(writeStream);
        }

        /// <summary>
        /// 初始化一个 <see cref="StreamContext"/> class 实例。
        /// </summary>
        /// <param name="spec">当前协议规范定义实例</param>
        /// <param name="exchangeStream">协议交互共享的字节序列</param>
        public StreamContext(SpecFile spec, Stream exchangeStream)
            : this(spec, exchangeStream, exchangeStream)
        {

        }

        BinaryWriter _writer = null;
        BinaryReader _reader = null;

        /// <summary>
        /// 获取或设置当前会话使用的协议定义文件实例
        /// </summary>
        public SpecFile ContractSpec { get; private set; }

        long _position = 0;
        /// <summary>
        /// 获取当前上下文的位置(默认索引位置为0)
        /// </summary>
        public long Position
        {
            get { return _position; }
        }

        /// <summary>
        /// 手工设置当前位置
        /// </summary>
        /// <param name="pos"></param>
        public void SetPosition(long pos)
        {
            _position = pos;
        }

        /// <summary>
        /// 增加指定长度的位置
        /// </summary>
        /// <param name="posAdd">需要增加的长度</param>
        public void IncreasePosition(long posAdd)
        {
            if (posAdd < 1)
            {
                throw new InvalidDataException("上下文递增位置不能小于1！");
            }
            _position += posAdd;
        }


        DataItem? _lastDataItem = null;
        /// <summary>
        /// 获取最近的上一个数据项
        /// </summary>
        /// <returns></returns>
        public DataItem? GetLastDataItem()
        {
            return _lastDataItem;
        }

        /// <summary>
        /// 获取或设置上下文即时绑定对象词典
        /// </summary>
        public Dictionary<string, object> ContextObjectDictionary { get; set; }

        /// <summary>
        /// 获取上下文请求对象的词典格式
        /// </summary>
        public Dictionary<string, object> GetContextRequest()
        {
            return ReadByContract(this, _reader, ContractSpec.RequestContract);
        }

        /// <summary>
        /// 根据协议规范输出当前应答对象数据
        /// </summary>
        /// <param name="respDict">应答对象的词典格式</param>
        public void WriteContextResponse(Dictionary<string, object> respDict)
        {
            WriteByContract(this, _writer, respDict, ContractSpec.ResponseContract);
        }

        #region IDisposable 成员

        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        public void Dispose()
        {
            if (_reader != null) _reader.Close();
            if (_writer != null) _writer.Close();
        }

        #endregion

        #region 静态辅助方法
        /// <summary>
        /// 在当前上下文中读取已知类型读取数据，并移动上下文读取游标索引。
        /// </summary>
        /// <param name="context">当前字节序列上下文</param>
        /// <param name="reader">二进制读取器</param>
        /// <param name="dType">规范数据定义</param>
        /// <returns></returns>
        public static object ReadKnownTypeObject(StreamContext context, BinaryReader reader, SpecDataType dType)
        {
            object itemObj = null;

            Type clrType = dType.GetRuntimeType();
            if (dType.Prototype == SpecDataPrototype.Native)
            {
                itemObj = SpecData.CLRObjectFromReader(clrType, reader);
                context.IncreasePosition(SpecData.GetCLRTypeByteLength(clrType));
            }
            else
            {
                if (dType.Prototype == SpecDataPrototype.Array)
                {
                    Type elementType = dType.GetElementType();
                    Array arrObj = Array.CreateInstance(elementType, dType.ElementLength);
                    if (SpecDataType.IsNativeType(TypeCache.ToSimpleType(elementType)))
                    {
                        #region 读取基础数据类型数组
                        long unitLen = SpecData.GetCLRTypeByteLength(elementType);
                        for (int i = 0, j = dType.ElementLength; i < j; i++)
                        {
                            arrObj.SetValue(SpecData.CLRObjectFromReader(elementType, reader), i);
                            context.IncreasePosition(unitLen);
                        }
                        #endregion
                    }
                    else
                    {
                        for (int m = 0, n = dType.ElementLength; m < n; m++)
                        {
                            arrObj.SetValue(ReadKnownTypeObject(context, reader, dType.GetElementDataType()), m);
                        }
                    }
                    itemObj = arrObj;
                }
                else
                {
                    #region 扩展读取
                    if (clrType.Equals(typeof(EnumContract)))
                    {
                        EnumContract enc = (EnumContract)dType.GetDefineInstance();
                        clrType = enc.GetBaseRuntimeType();
                        itemObj = SpecData.CLRObjectFromReader(clrType, reader);
                        context.IncreasePosition(SpecData.GetCLRTypeByteLength(clrType));
                    }
                    else if (clrType.Equals(typeof(DataContract)))
                    {
                        DataContract dac = (DataContract)dType.GetDefineInstance();
                        itemObj = ReadByContract(context, reader, dac);
                    }
                    else
                    {
                        throw new SpecDataDefineException(string.Format("不能读取数据类型{0}！", clrType.FullName));
                    }
                    #endregion
                }
            }

            return itemObj;
        }

        /// <summary>
        /// 在当前上下文中写入已知类型数据，并移动上下文写入游标索引。
        /// </summary>
        /// <param name="context">当前字节序列上下文</param>
        /// <param name="writer">二进制写入器</param>
        /// <param name="dType">规范数据定义</param>
        /// <param name="objData">写入对象实例</param>
        /// <param name="isNetworkBytes">是否写入网络序</param>
        public static void WriteKnownTypeObject(StreamContext context, BinaryWriter writer, SpecDataType dType, object objData, bool isNetworkBytes)
        {
            Type clrType = dType.GetRuntimeType();
            if (dType.Prototype == SpecDataPrototype.Native)
            {
                context.IncreasePosition(SpecData.CLRObjectWrite(clrType, writer, objData, isNetworkBytes));
            }
            else
            {
                if (dType.Prototype == SpecDataPrototype.Array)
                {
                    Type elementType = dType.GetElementType();
                    Array arrObj = (Array)objData;
                    if (SpecDataType.IsNativeType(TypeCache.ToSimpleType(elementType)))
                    {
                        #region 写入基础数据类型数组
                        for (int i = 0, j = dType.ElementLength; i < j; i++)
                        {
                            context.IncreasePosition(SpecData.CLRObjectWrite(elementType, writer, arrObj.GetValue(i), isNetworkBytes));
                        }
                        #endregion
                    }
                    else
                    {
                        for (int m = 0, n = dType.ElementLength; m < n; m++)
                        {
                            WriteKnownTypeObject(context, writer, dType.GetElementDataType(), arrObj.GetValue(m), isNetworkBytes);
                        }
                    }
                }
                else
                {
                    #region 扩展写入
                    if (clrType.Equals(typeof(EnumContract)))
                    {
                        EnumContract enc = (EnumContract)dType.GetDefineInstance();
                        clrType = enc.GetBaseRuntimeType();
                        if (!objData.GetType().Equals(clrType))
                        {
                            objData = enc.GetEnumUnderlyingValue(objData.ToString());
                        }
                        context.IncreasePosition(SpecData.CLRObjectWrite(clrType, writer, objData, isNetworkBytes));
                    }
                    else if (clrType.Equals(typeof(DataContract)))
                    {
                        DataContract dac = (DataContract)dType.GetDefineInstance();
                        WriteByContract(context, writer, (Dictionary<string, object>)objData, dac);
                    }
                    else
                    {
                        throw new SpecDataDefineException(string.Format("不能读取数据类型{0}！", clrType.FullName));
                    }
                    #endregion
                }
            }
        }

        /// <summary>
        /// 按照数据规范读取数据为数据词典
        /// </summary>
        /// <param name="context">当前字节序列上下文</param>
        /// <param name="reader">二进制读取器</param>
        /// <param name="contracts">协议规范</param>
        /// <returns></returns>
        public static Dictionary<string, object> ReadByContract(StreamContext context, BinaryReader reader, DataContract contracts)
        {
            Dictionary<string, object> reqDict = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
            context.ContextObjectDictionary = reqDict;
            foreach (DataItem item in contracts.TransItems)
            {
                #region 判断是否读取当前项
                if (!string.IsNullOrEmpty(item.ConditionalExpression))
                {
                    if (!new SpecExpression(item.ConditionalExpression).IsPass(context))
                        continue;
                }
                #endregion
                SpecDataType dType = SpecDataType.Parse(context, item.DataType);
                Type clrType = dType.GetRuntimeType();
                if (clrType == null)
                {
                    throw new SpecDataDefineException(string.Format("数据类型{0}不能识别！", item.DataType));
                }
                else
                {
                    reqDict[item.DataName] = ReadKnownTypeObject(context, reader, dType);
                }

                //设置最近一个解析项
                context._lastDataItem = item;
            }
            return reqDict;
        }

        /// <summary>
        /// 按照数据规范输出数据
        /// </summary>
        /// <param name="context">当前字节序列上下文</param>
        /// <param name="writer">二进制写入器</param>
        /// <param name="respDict">输出数据词典</param>
        /// <param name="contracts">当前协议规范</param>
        public static void WriteByContract(StreamContext context, BinaryWriter writer, Dictionary<string, object> respDict, DataContract contracts)
        {
            context.ContextObjectDictionary = respDict;
            foreach (DataItem item in contracts.TransItems)
            {
                #region 判断是否写入当前项
                if (!string.IsNullOrEmpty(item.ConditionalExpression))
                {
                    if (!new SpecExpression(item.ConditionalExpression).IsPass(context))
                        continue;
                }
                #endregion
                SpecDataType dType = SpecDataType.Parse(context, item.DataType);
                Type clrType = dType.GetRuntimeType();
                if (clrType == null)
                {
                    throw new SpecDataDefineException(string.Format("数据类型{0}不能识别！", item.DataType));
                }
                else
                {
                    WriteKnownTypeObject(context, writer, dType, respDict[item.DataName], item.NetworkBytes);
                }
                //设置最近一个解析项
                context._lastDataItem = item;
            }
        }

        #endregion

    }
}

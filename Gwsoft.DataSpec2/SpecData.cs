using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Gwsoft.DataSpec2
{
    /// <summary>
    /// 规范数据读取辅助类
    /// </summary>
    public static class SpecData
    {
        /// <summary>
        /// 根据数据字段定义从字节序列视图中读取特定类型的数据
        /// </summary>
        /// <typeparam name="TData">目标数据类型</typeparam>
        /// <param name="stmContext">源字节序列视图上下文</param>
        /// <param name="itemDef">数据项定义</param>
        /// <returns></returns>
        //public static TData Read<TData>(StreamContext stmContext, DataItem itemDef)
        //{
        //    return default(TData);
        //}

        /// <summary>
        /// 读取为指定类型的数据
        /// </summary>
        /// <typeparam name="TData">目标数据类型</typeparam>
        /// <param name="srcDict">当前数据词典</param>
        /// <param name="key">要读取的键值</param>
        /// <returns></returns>
        public static TData ReadAs<TData>(Dictionary<string, object> srcDict, string key)
        {
            if (!srcDict.ContainsKey(key))
            {
                return default(TData);
            }
            else
            {
                return (TData)srcDict[key];
            }
        }

        /// <summary>
        /// 转换到特定数值类型
        /// </summary>
        public static TData To<TData>(string rawString)
        {
            Converter<string, TData> gConvert = new Converter<string, TData>(s =>
            {
                if (rawString == null)
                {
                    return default(TData);
                }
                else
                {
                    return (TData)Convert.ChangeType(s, typeof(TData));
                }
            });
            return gConvert(rawString);
        }

        /// <summary>
        /// 无异常转换到特定数值类型
        /// </summary>
        public static TData As<TData>(string rawString)
        {
            TData result = default(TData);
            try
            {
                result = To<TData>(rawString);
            }
            catch { }
            return result;
        }

        /// <summary>
        /// 反转二进制字节序列
        /// </summary>
        public static byte[] ReverseBytes(byte[] bytes)
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
        /// 获取二进制存储的字节长度(BitConvert类兼容的字节长度)
        /// <para>返回-1则长度未知</para>
        /// </summary>
        public static long GetCLRTypeByteLength(Type DataType)
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
        /// 从二进制序列读取数据
        /// </summary>
        /// <param name="codeData">二进制序列</param>
        /// <param name="reverseBytes">是否反转字节序列</param>
        /// <param name="rdrFun">读取解析函数</param>
        /// <returns></returns>
        static object ObjectFromBytes(byte[] codeData, bool reverseBytes, ObjectBinaryReadHandler rdrFun)
        {
            object objRet = null;
            if (reverseBytes) codeData = ReverseBytes(codeData);
            using (MemoryStream ms = new MemoryStream(codeData))
            {
                ms.Position = 0;
                BinaryReader rdr = new BinaryReader(ms);
                objRet = rdrFun(rdr);
                rdr.Close();
            }
            return objRet;
        }

        /// <summary>
        /// 从读取器读取特定对象数据
        /// </summary>
        /// <param name="dataType">目标数据类型</param>
        /// <param name="reader">二进制读取器</param>
        /// <returns></returns>
        public static object CLRObjectFromReader(Type dataType, BinaryReader reader)
        {
            object objRet = null;
            TypeCode cTCode = Type.GetTypeCode(dataType);
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
                default:
                    throw new InvalidOperationException("类型" + dataType + "暂不支持!");
            }

            return objRet;
        }

        /// <summary>
        /// 从字节序列视图读取对象
        /// </summary>
        /// <param name="dataType">目标数据类型</param>
        /// <param name="ntkStm">序列视图读取源</param>
        /// <returns></returns>
        public static object CLRObjectFromStream(Type dataType, Stream ntkStm)
        {
            BinaryReader reader = new BinaryReader(ntkStm);
            return CLRObjectFromReader(dataType, reader);
        }

        /// <summary>
        /// 从当前位置写入对象字节数据，并返回前进的字节长度
        /// </summary>
        /// <param name="dataType">目标数据类型</param>
        /// <param name="writer">数据写入器</param>
        /// <param name="objData">当前数据实例</param>
        /// <param name="isNetworkBytes">是否写入网络序</param>
        /// <returns></returns>
        public static long CLRObjectWrite(Type dataType, BinaryWriter writer, object objData, bool isNetworkBytes)
        {
            byte[] binBytes = new byte[0];
            long byteLen = 0;
            TypeCode cTCode = Type.GetTypeCode(dataType);
            switch (cTCode)
            {
                case TypeCode.Boolean:
                    byteLen = 1;
                    writer.Write((bool)objData);
                    break;
                case TypeCode.Byte:
                    byteLen = 1;
                    writer.Write((byte)objData);
                    break;
                case TypeCode.SByte:
                    byteLen = 1;
                    writer.Write((sbyte)objData);
                    break;

                case TypeCode.Int16:
                    byteLen = 2;
                    binBytes = BitConverter.GetBytes((short)objData);
                    break;
                case TypeCode.UInt16:
                    byteLen = 2;
                    binBytes = BitConverter.GetBytes((ushort)objData);
                    break;

                case TypeCode.Char:
                    byteLen = 4;
                    binBytes = BitConverter.GetBytes((int)((char)objData));
                    break;
                case TypeCode.Int32:
                    byteLen = 4;
                    binBytes = BitConverter.GetBytes((int)objData);
                    break;
                case TypeCode.UInt32:
                    byteLen = 4;
                    binBytes = BitConverter.GetBytes((uint)objData);
                    break;
                case TypeCode.Single:
                    byteLen = 4;
                    binBytes = BitConverter.GetBytes((float)objData);
                    break;

                case TypeCode.Int64:
                    byteLen = 8;
                    binBytes = BitConverter.GetBytes((long)objData);
                    break;
                case TypeCode.UInt64:
                    byteLen = 8;
                    binBytes = BitConverter.GetBytes((ulong)objData);
                    break;
                case TypeCode.Double:
                    byteLen = 8;
                    binBytes = BitConverter.GetBytes((double)objData);
                    break;

                case TypeCode.DBNull:
                    break;
                case TypeCode.DateTime:
                    break;
                case TypeCode.Decimal:
                    break;
                case TypeCode.Empty:
                    break;
                case TypeCode.String:
                    break;
                case TypeCode.Object:
                    break;
                default:
                    break;
            }

            if (binBytes.Length > 0)
            {
                if (isNetworkBytes) binBytes = ReverseBytes(binBytes);
                writer.Write(binBytes);
            }
            return byteLen;
        }

    }

    /// <summary>
    /// 获取节点数据项描述的委托
    /// </summary>
    /// <param name="contractName">节点名称</param>
    /// <param name="itemName">项名称</param>
    /// <returns></returns>
    public delegate string DataItemCommentFetchHanlder(string contractName, string itemName);

    /// <summary>
    /// 二进制读取数据到对象委托
    /// </summary>
    /// <param name="reader">二进制数据读取器</param>
    /// <returns></returns>
    public delegate object ObjectBinaryReadHandler(BinaryReader reader);
}

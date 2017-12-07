using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Gwsoft.DataSpec2
{
    /// <summary>
    /// 配置服务的类型缓存
    /// </summary>
    public static class TypeCache
    {
        static readonly Dictionary<string, Type> FTD = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// 直接构建新实例 <see cref="ConfigService"/> class.
        /// </summary>
        static TypeCache()
        {
            FTD.Add("string", typeof(string));
            FTD.Add("string[]", typeof(string[]));
            FTD.Add("bool", typeof(bool));

            FTD.Add("int", typeof(System.Int32));
            FTD.Add("int[]", typeof(System.Int32[]));
            FTD.Add("uint", typeof(UInt32));

            FTD.Add("ulong", typeof(UInt64));
            FTD.Add("long", typeof(System.Int64));
            FTD.Add("long[]", typeof(System.Int64[]));

            FTD.Add("ushort", typeof(UInt16));
            FTD.Add("short", typeof(System.Int16));
            FTD.Add("short[]", typeof(System.Int16[]));

            FTD.Add("char", typeof(char));
            FTD.Add("byte", typeof(byte));
            FTD.Add("sbyte", typeof(SByte));
            FTD.Add("byte[]", typeof(byte[]));

            FTD.Add("float", typeof(float));
            FTD.Add("decimal", typeof(decimal));
            FTD.Add("double", typeof(double));

            FTD.Add("datetime", typeof(System.DateTime));

            #region 常用复合类型
            FTD.Add("Dictionary<string,object>", typeof(Dictionary<string, object>));
            FTD.Add("Hashtable", typeof(Hashtable));
            FTD.Add("NameValueCollection", typeof(System.Collections.Specialized.NameValueCollection));
            #endregion
        }

        /// <summary>
        /// 获取运行时类型
        /// </summary>
        /// <param name="typeIdString">类型标识</param>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">System.Configuration.ConfigurationErrorsException</exception>
        /// <returns></returns>
        public static Type GetRuntimeType(string typeIdString)
        {
            int gsIdx = typeIdString.IndexOf('<');
            int geIdx = typeIdString.LastIndexOf('>');
            if (gsIdx != -1 && geIdx > gsIdx)
            {
                //NetTask.Core.ScopeItemCompare<int>, NetTask.Interface
                //NetTask.Core.ScopeItemCompare`1[[System.Int32, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]
                #region 友好的泛型字符
                string gstr = typeIdString.Substring(gsIdx, geIdx - gsIdx + 1);
                string[] gtArr = gstr.Substring(1, gstr.Length - 2).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                string gdStr = string.Concat(typeIdString.Substring(0, gsIdx),
                    "`" + gtArr.Length,
                    typeIdString.Substring(geIdx + 1));

                Type baseT = Type.GetType(gdStr, true);
                List<Type> garr = new List<Type>();
                foreach (string ga in gtArr)
                {
                    garr.Add(GetRuntimeType(ga));
                }
                return baseT.MakeGenericType(garr.ToArray());
                #endregion
            }
            else
            {
                if (FTD.ContainsKey(typeIdString)) return FTD[typeIdString];
                Type stepType = Type.GetType(typeIdString, false);
                if (stepType == null)
                {
                    throw new System.Configuration.ConfigurationErrorsException(string.Format("配置类型{0}未找到!", typeIdString));
                }
                return stepType;
            }
        }

        /// <summary>
        /// 转换为简易类型
        /// </summary>
        /// <param name="runtimeType">运行时类型</param>
        /// <returns></returns>
        public static string ToSimpleType(Type runtimeType)
        {
            KeyValuePair<string, Type> existPair = FirstOrDefault<KeyValuePair<string, Type>>(FTD, t => t.Value.Equals(runtimeType));
            if (existPair.Key != null)
            {
                return existPair.Key;
            }
            else
            {
                return GetNoVersionTypeName(runtimeType);
            }
        }

        /// <summary>
        /// 返回序列中满足条件的第一个元素；如果未找到这样的元素，则返回默认值。
        /// </summary>
        /// <typeparam name="TData">source 中的元素的类型。</typeparam>
        /// <param name="source">要返回其第一个元素的 System.Collections.Generic.IEnumerable&lt;TData&gt;。</param>
        /// <param name="match">用于测试每个元素是否满足条件的函数。</param>
        /// <returns></returns>
        public static TData FirstOrDefault<TData>(IEnumerable<TData> source, Predicate<TData> match)
        {
            TData ret = default(TData);
            if (source != null)
            {
                IEnumerator<TData> er = source.GetEnumerator();
                while (er.MoveNext())
                {
                    if (match(er.Current))
                    {
                        ret = er.Current;
                        break;
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// 获取不包含版本的类型全称，形如：BizService.Interface.Services.LogService, BizService.Interface。
        /// </summary>
        /// <param name="instanceType">对象类型</param>
        /// <returns></returns>
        public static string GetNoVersionTypeName(Type instanceType)
        {
            if (!instanceType.IsGenericType)
            {
                return string.Format("{0}, {1}",
                instanceType.FullName,
                Path.GetFileNameWithoutExtension(instanceType.Assembly.Location));
            }
            else
            {
                string rawFullName = instanceType.FullName;
                string baseTypeName = rawFullName.Substring(0, rawFullName.IndexOf('`'));
                return string.Format("{0}<{1}>, {2}",
                    baseTypeName,
                    string.Join(",", Array.ConvertAll<Type, string>(instanceType.GetGenericArguments(),
                    t => ToSimpleType(t))),
                Path.GetFileNameWithoutExtension(instanceType.Assembly.Location));
            }

        }

        /// <summary>
        /// 二进制序列的16进制视图形式（16字节换行）
        /// </summary>
        public static string ByteArrayToHexString(byte[] tBinBytes)
        {
            string draftStr = System.Text.RegularExpressions.Regex.Replace(BitConverter.ToString(tBinBytes),
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
        public static byte[] HexPatternStringToByteArray(string hexrawStr)
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
                        System.Globalization.NumberStyles.AllowHexSpecifier));
                }
                return rawBin;
            }
        }

        /// <summary>
        /// 转换为字符串形式的值
        /// </summary>
        /// <param name="objVal"></param>
        /// <returns></returns>
        public static string ToStringValue(object objVal)
        {
            if (objVal == null)
            {
                return null;
            }
            else
            {
                Type valType = objVal.GetType();
                if (valType.Equals(typeof(byte[])))
                {
                    return ByteArrayToHexString((byte[])objVal);
                }
                else
                {
                    return objVal.ToString();
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
//using Gwsoft.DataSpec2.Util;

namespace Gwsoft.DataSpec2
{
    /// <summary>
    /// 枚举协议定义
    /// </summary>
    [Serializable]
    [System.Diagnostics.DebuggerDisplay("{TypeName,nq} : {BaseType,nq}")]
    public class EnumContract : ISpecObject
    {
        /// <summary>
        /// 获取或设置枚举类型名称
        /// </summary>
        [XmlAttribute]
        public string TypeName { get; set; }

        /// <summary>
        /// 获取或设置枚举类型的描述
        /// </summary>
        [XmlAttribute]
        public string Description { get; set; }

        /// <summary>
        /// 获取或设置基础数字类型
        /// </summary>
        [XmlAttribute]
        public string BaseType { get; set; }

        /// <summary>
        /// 获取改枚举的基础运行时类型
        /// </summary>
        /// <returns></returns>
        public Type GetBaseRuntimeType()
        {
            return TypeCache.GetRuntimeType(BaseType);
        }

        DataItem[] _allDef = new DataItem[0];
        /// <summary>
        /// 获取或设置所有定义(符合规范的枚举定义至少有一个或以上的定义)
        /// </summary>
        public DataItem[] Definition
        {
            get { return _allDef; }
            set { _allDef = value; }
        }

        /// <summary>
        /// 获取枚举字段定义，如果没有改字段定义则为null。
        /// </summary>
        /// <param name="itemName">字段名称</param>
        /// <returns></returns>
        public DataItem? this[string itemName]
        {
            get
            {
                if (_allDef == null || _allDef.Length < 1)
                {
                    return null;
                }
                else
                {
                    for (int i = 0, j = _allDef.Length; i < j; i++)
                    {
                        if (_allDef[i].DataName.Equals(itemName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            return _allDef[i];
                        }
                    }
                    return null;
                }
            }
            set
            {
                if (value != null)
                {
                    int idx = Array.FindIndex<DataItem>(_allDef, t => t.DataName.Equals(itemName, StringComparison.InvariantCultureIgnoreCase));
                    if (idx != -1)
                    {
                        if (value.HasValue)
                        {
                            _allDef[idx] = value.Value;
                        }
                        else
                        {
                            #region 删除枚举定义项
                            List<DataItem> newItemList = new List<DataItem>();
                            for (int m = 0, n = _allDef.Length; m < n; m++)
                            {
                                if (m == idx)
                                {
                                    continue;
                                }
                                else
                                {
                                    newItemList.Add(_allDef[m]);
                                }
                            }
                            _allDef = newItemList.ToArray();
                            #endregion
                        }
                    }
                    else
                    {
                        if (value.HasValue)
                        {
                            DataItem[] newItems = new DataItem[_allDef.Length + 1];
                            Array.Copy(_allDef, newItems, _allDef.Length);
                            newItems[newItems.Length - 1] = value.Value;

                            _allDef = newItems;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获取枚举定义项的真实数据值
        /// </summary>
        /// <param name="enumItemName">枚举定义项名称</param>
        /// <returns></returns>
        public object GetEnumUnderlyingValue(string enumItemName)
        {
            DataItem? item = this[enumItemName];
            if (item == null)
            {
                return null;
            }
            else
            {
                return GetUnderlyingValue(item.Value.ItemValue);
            }
        }

        /// <summary>
        /// 直接设置枚举项的值
        /// </summary>
        /// <param name="enumItemName">枚举定义项名称</param>
        /// <param name="itemVal">改项的值</param>
        public void SetEnumItem(string enumItemName, object itemVal)
        {
            this[enumItemName] = new DataItem { DataName = enumItemName, ItemValue = itemVal };
        }

        /// <summary>
        /// 获取枚举的实际数据类型
        /// </summary>
        /// <param name="defineVal">定义值</param>
        /// <returns></returns>
        public object GetUnderlyingValue(object defineVal)
        {
            if (defineVal == null)
            {
                throw new ArgumentNullException("defineVal");
            }

            string valType = TypeCache.ToSimpleType(defineVal.GetType());
            if (BaseType.Equals(valType))
            {
                return defineVal;
            }
            else
            {
                //从字符串转换为目标类型
                return Convert.ChangeType(defineVal.ToString(), TypeCache.GetRuntimeType(BaseType));
            }
        }

        /// <summary>
        /// 判定指定值是否等于特定枚举定义项
        /// </summary>
        /// <param name="objVal">需要比较的值</param>
        /// <param name="enumItem">枚举项值定义</param>
        /// <returns></returns>
        public static bool DataEquals(object objVal, DataItem enumItem)
        {
            if (objVal == null || enumItem.ItemValue == null)
            {
                return false;
            }
            else
            {
                return objVal.Equals(enumItem.ItemValue);
            }
        }

        /// <summary>
        /// 解析为枚举项数据
        /// </summary>
        /// <param name="name">枚举定义名称</param>
        /// <param name="description">枚举定义描述</param>
        /// <param name="defRaw">原始定义字符串</param>
        /// <returns></returns>
        public static EnumContract Parse(string name, string description, string defRaw)
        {
            EnumContract enc = new EnumContract();
            //EaseSuccessFlag = short=>{Error:-1, UnKnown:0, Success:1010, SuccessUserAgent:1020, SuccessExpress:1120}
            //#服务器响应码(short 3.2-2.6.9)
            //StatusCode=short=>{Exception:-1/*服务器异常*/, Success:0/*服务器处理成功*/, Updatable:1/*主程序有更新,请按照客户端更新策略下载*/}

            enc.TypeName = name;
            enc.Description = description;
            int idx = defRaw.IndexOf("=>");
            if (idx == -1)
            {
                throw new SpecDataDefineException("没有找到枚举数据类型定义的分隔标识符号:'=>'!");
            }
            enc.BaseType = defRaw.Substring(0, idx).Trim();
            string itemDefExp = defRaw.Substring(idx + 2).Trim().Trim('{', '}');

            #region 最简定义解析
            string[] itemArr = itemDefExp.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            string key = null, value = null;
            foreach (string item in itemArr)
            {
                idx = item.IndexOf(':');
                if (idx == -1)
                {
                    continue;
                }
                else
                {
                    key = item.Substring(0, idx).Trim();
                    value = item.Substring(idx + 1);

                    //检查是否包含注释
                    idx = value.IndexOf('/');
                    if (idx == -1)
                    {
                        enc.SetEnumItem(key, value);
                    }
                    else
                    {
                        enc[key] = new DataItem
                        {
                            DataName = key,
                            ItemValue = value.Substring(0, idx).Trim(),
                            Description = value.Substring(idx).Trim('/', '*')
                        };
                    }
                }
            }
            #endregion

            #region 灵活定义解析
            //using (StringParseHelper dep = new StringParseHelper(itemDefExp))
            //{
            //    //{Exception:-1/*服务器异常*/, Success:0/*服务器处理成功*/, Updatable:1/*主程序有更新,请按照客户端更新策略下载*/}
            //    /*
            //     * when found : => getBufferAsKey -> readToChar([',','}']) -> getBufferAsValue
            //     * when found / => checkNextCharIs('*') -> readToChar(['*']) and peekNextChar == '/' as Key Comment
            //     * 
            //     */
            //    dep.Parse();
            //}
            #endregion

            return enc;
        }

#if TEST

        public void ParseTest()
        {

            EnumContract ec = EnumContract.Parse("StatusCode", "服务器响应码(short 3.2-2.6.9)",
                "short=>{Exception:-1/*服务器异常*/, Success:0/*服务器处理成功*/, Updatable:1/*主程序有更新 请按照客户端更新策略下载*/}");

            Console.WriteLine(ec.Definition.Length);

        }

#endif

    }
}

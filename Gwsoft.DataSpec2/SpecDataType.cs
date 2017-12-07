using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Gwsoft.DataSpec2
{
    /// <summary>
    /// 契约数据类型
    /// </summary>
    [Serializable]
    public class SpecDataType
    {
        /// <summary>
        /// 数据原型
        /// </summary>
        [XmlAttribute]
        public SpecDataPrototype Prototype { get; set; }

        /// <summary>
        /// 数据类型名称
        /// </summary>
        [XmlAttribute]
        public string TypeName { get; set; }

        /// <summary>
        /// 获取或设置数据项长度(仅对于数组类型)
        /// </summary>
        [XmlAttribute]
        public int ElementLength { get; set; }

        Type _runtimeType = null;
        /// <summary>
        /// 设置改数据类型的运行时类型
        /// </summary>
        /// <param name="runtimeType">运行时类型</param>
        public void SetRuntimeType(Type runtimeType)
        {
            _runtimeType = runtimeType;
        }

        Type _elementType = null;
        /// <summary>
        /// 获取数组类型的单元项数据类型
        /// </summary>
        /// <returns></returns>
        public Type GetElementType()
        {
            return _elementType;
        }

        /// <summary>
        /// 获取当前运行时数据类型
        /// </summary>
        /// <returns></returns>
        public Type GetRuntimeType()
        {
            return _runtimeType;
        }

        object typeDefineInstance = null;
        /// <summary>
        /// 获取当前数据类型定义的实例
        /// </summary>
        /// <returns></returns>
        public object GetDefineInstance()
        {
            return typeDefineInstance;
        }

        /// <summary>
        /// 元素类型定义
        /// </summary>
        SpecDataType eleSpecDataType = null;
        /// <summary>
        /// 获取数据定义的元素项定义
        /// </summary>
        /// <returns></returns>
        public SpecDataType GetElementDataType()
        {
            return eleSpecDataType;
        }

        /// <summary>
        /// 原生内置支持的数据类型
        /// </summary>
        public const string NativeClrTypes = "byte,sbyte,bool,char,double,single,short,ushort,int,uint,long,ulong";

        /// <summary>
        /// 判断类型定义是否是基础原生类型
        /// </summary>
        /// <param name="typeDef">类型定义</param>
        /// <returns></returns>
        public static Boolean IsNativeType(string typeDef)
        {
            return ("," + NativeClrTypes + ",").IndexOf(("," + typeDef + ",")) != -1;
        }

        /// <summary>
        /// 解析数据类型
        /// </summary>
        /// <param name="context">当前解析上下文</param>
        /// <param name="rawTypeDefine">原始定义字符串</param>
        /// <returns></returns>
        public static SpecDataType Parse(StreamContext context, string rawTypeDefine)
        {
            SpecDataType sType = new SpecDataType();
            sType.TypeName = rawTypeDefine;

            int idx = -1, idx2 = -1;
            idx = rawTypeDefine.IndexOf('[');
            idx2 = rawTypeDefine.IndexOf(']');
            if (idx != -1 && idx2 != -1)
            {
                if (idx2 - idx == 1)
                {
                    throw new SpecDataDefineException("数组类型定义[" + rawTypeDefine + "]错误，[]必须包含数据项长度或使用上下文计算的-1!");
                }

                sType.Prototype = SpecDataPrototype.Array;

                string strTemp = rawTypeDefine.Substring(0, idx);
                SpecDataType esType = Parse(context, strTemp);
                sType._elementType = esType.GetRuntimeType();
                sType.eleSpecDataType = esType;

                strTemp = rawTypeDefine.Substring(idx + 1, idx2 - idx - 1).Trim();
                if (strTemp == "-1")
                {
                    DataItem? item = context.GetLastDataItem();
                    if (!item.HasValue)
                    {
                        throw new SpecDataDefineException("数组类型定义[-1]在上下文中没有前置传输定义项!");
                    }
                    else
                    {
                        sType.ElementLength = Convert.ToInt32(context.ContextObjectDictionary[item.Value.DataName]);
                    }
                }
                else
                {
                    if (!context.ContextObjectDictionary.ContainsKey(strTemp))
                    {
                        throw new SpecDataDefineException("数组长度定义[" + strTemp + "]在上下文的没有找到长度定义!");
                    }
                    else
                    {
                        sType.ElementLength = Convert.ToInt32(context.ContextObjectDictionary[strTemp]);
                    }
                }
                sType.SetRuntimeType(sType._elementType.MakeArrayType());
            }
            else
            {
                #region 非数组定义
                if (IsNativeType(rawTypeDefine))
                {
                    sType.Prototype = SpecDataPrototype.Native;
                    sType.SetRuntimeType(TypeCache.GetRuntimeType(rawTypeDefine));
                }
                else
                {
                    SpecFile specDef = context.ContractSpec;
                    EnumContract enc = TypeCache.FirstOrDefault<EnumContract>(specDef.AllDefinition, d => d.TypeName.Equals(rawTypeDefine));
                    if (enc != null)
                    {
                        sType.Prototype = SpecDataPrototype.Enum;
                        sType.SetRuntimeType(typeof(EnumContract));
                        sType.typeDefineInstance = enc;
                    }
                    else
                    {
                        DataContract dac = TypeCache.FirstOrDefault<DataContract>(specDef.AllImportContracts, c => c.ContractName.Equals(rawTypeDefine));
                        if (dac == null)
                        {
                            throw new SpecDataDefineException("类型定义" + rawTypeDefine + "不能识别!");
                        }
                        sType.Prototype = SpecDataPrototype.Composite;
                        sType.SetRuntimeType(typeof(DataContract));
                        sType.typeDefineInstance = dac;
                    }
                }
                #endregion
            }
            return sType;
        }
    }

    /// <summary>
    /// 规则数据原型
    /// </summary>
    public enum SpecDataPrototype : byte
    {
        /// <summary>
        /// 原生数据类型，如byte,int,long,short等
        /// </summary>
        [Description("原生数据类型，如byte,int,long,short等")]
        Native = 0,
        /// <summary>
        /// 自定义的枚举类型
        /// </summary>
        Enum = 1,
        /// <summary>
        /// 符合类型，自定义类（数据契约）
        /// </summary>
        Composite = 2,
        /// <summary>
        /// 数组
        /// </summary>
        Array = 3
    }
}

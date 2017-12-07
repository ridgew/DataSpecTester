using System;
using System.Xml.Serialization;
using System.IO;
using System.Collections.Generic;

namespace Gwsoft.DataSpec2
{
    /// <summary>
    /// 规则文件读取
    /// </summary>
    [Serializable]
    [System.Diagnostics.DebuggerDisplay("{FileName}")]
    public class SpecFile
    {
        /// <summary>
        /// 文件名称
        /// </summary>
        [XmlAttribute]
        public string FileName { get; set; }

        /// <summary>
        /// 文件资源地址
        /// </summary>
        [XmlIgnore]
        public string FileUrl { get; set; }

        /// <summary>
        /// 文件描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 数据请求协议(上行数据包)
        /// </summary>
        public DataContract RequestContract { get; set; }

        /// <summary>
        /// 数据响应协议(下行数据包)
        /// </summary>
        public DataContract ResponseContract { get; set; }

        /// <summary>
        /// 获取或设置所有枚举定义
        /// </summary>
        public EnumContract[] AllDefinition { get; set; }

        /// <summary>
        /// 获取或设置所有导入的数据契约
        /// </summary>
        public DataContract[] AllImportContracts { get; set; }

        /// <summary>
        /// 获取特定名称的导入契约定义，如果没有找到则为null。
        /// </summary>
        /// <param name="contractName">契约名称</param>
        /// <returns></returns>
        public DataContract this[string contractName]
        {
            get
            {
                return TypeCache.FirstOrDefault<DataContract>(AllImportContracts, d => d.ContractName.Equals(contractName));
            }
        }

        /// <summary>
        /// 获取改文件中的枚举定义，如果没有找到则为null。
        /// </summary>
        /// <param name="defName">枚举名称</param>
        /// <returns></returns>
        public EnumContract GetDefine(string defName)
        {
            return TypeCache.FirstOrDefault<EnumContract>(AllDefinition, d => d.TypeName.Equals(defName));
        }

        /// <summary>
        /// 所有导入规范数据词典
        /// </summary>
        Dictionary<string, SpecFile> ImportSpecDict = new Dictionary<string, SpecFile>();

        /// <summary>
        /// 合并导入规范数据
        /// </summary>
        /// <param name="impSpec">规范数据文件实例</param>
        public void MerginWithSpecFile(SpecFile impSpec)
        {
            if (ImportSpecDict.ContainsKey(impSpec.FileUrl))
            {
                return;
            }
            else
            {
                int i = 0, j = 0;
                #region 导入枚举定义
                if (impSpec.AllDefinition != null && impSpec.AllDefinition.Length > 0)
                {
                    List<EnumContract> enumList = new List<EnumContract>();

                    EnumContract current = null;
                    for (i = 0, j = impSpec.AllDefinition.Length; i < j; i++)
                    {
                        current = impSpec.AllDefinition[i];
                        if (GetDefine(current.TypeName) != null)
                        {
                            continue;
                        }
                        else
                        {
                            enumList.Add(current);
                        }
                    }

                    #region 导入现有定义
                    if (AllDefinition != null) enumList.AddRange(AllDefinition);
                    #endregion
                    AllDefinition = enumList.ToArray();
                }
                #endregion

                #region 导入数据契约定义
                if (impSpec.AllImportContracts != null && impSpec.AllImportContracts.Length > 0)
                {
                    List<DataContract> impList = new List<DataContract>();

                    DataContract crtContract = null;
                    for (i = 0, j = impSpec.AllImportContracts.Length; i < j; i++)
                    {
                        crtContract = impSpec.AllImportContracts[i];
                        if (this[crtContract.ContractName] != null)
                        {
                            continue;
                        }
                        else
                        {
                            impList.Add(crtContract);
                        }
                    }

                    #region 导入现有契约定义
                    if (AllImportContracts != null) impList.AddRange(AllImportContracts);
                    #endregion

                    AllImportContracts = impList.ToArray();
                }
                #endregion
                ImportSpecDict.Add(impSpec.FileUrl, impSpec);
            }
        }

        /// <summary>
        /// 合并并排序定义集合
        /// </summary>
        /// <param name="defList">需要合并的定义集合</param>
        public void MerginDefineWith(List<EnumContract> defList)
        {
            if (AllDefinition != null && AllDefinition.Length > 0)
            {
                defList.AddRange(AllDefinition);
            }
            EnumContract[] defArr = defList.ToArray();
            Array.Sort<EnumContract>(defArr, new Comparison<EnumContract>((a, b) =>
            {
                return a.TypeName.CompareTo(b.TypeName);
            }));
            AllDefinition = defArr;
        }

        /// <summary>
        /// 合并并排序导入集合
        /// </summary>
        /// <param name="impList">需要合并的导入集合</param>
        public void MerginImportWith(List<DataContract> impList)
        {
            if (AllImportContracts != null && AllImportContracts.Length > 0)
            {
                impList.AddRange(AllImportContracts);
            }
            DataContract[] impArr = impList.ToArray();
            Array.Sort<DataContract>(impArr, new Comparison<DataContract>((a, b) =>
            {
                return a.ContractName.CompareTo(b.ContractName);
            }));
            AllImportContracts = impArr;
        }

        /// <summary>
        /// 导入并合并现有的契约规范定义
        /// </summary>
        /// <param name="srcSpecFile">原始请求规范文件定义</param>
        /// <param name="specDef">规范文件路径或名称，相对路径。</param>
        public static void ImportSpecFile(SpecFile srcSpecFile, string specDef)
        {
            if (!specDef.EndsWith(".spec")) specDef += ".spec";

            string impnewFile = Path.Combine(Path.GetDirectoryName(srcSpecFile.FileUrl), specDef);
            SpecFile newSpec = ParseContractFile(impnewFile, SpecFileFormat.Ini);
            srcSpecFile.MerginWithSpecFile(newSpec);
        }

        /// <summary>
        /// [ST:DEBUG]读取配置项词典为数据契约定义
        /// </summary>
        /// <param name="srcSpecFile">原始请求规范文件定义</param>
        /// <param name="contractName">数据契约名称</param>
        /// <param name="itemDict">配置项词典</param>
        /// <param name="cmtHandler">获取项描述的委托</param>
        /// <param name="importHandler">导入(import)规范委托</param>
        /// <returns></returns>
        static DataContract ReadAsContract(SpecFile srcSpecFile, string contractName, Dictionary<string, string> itemDict, DataItemCommentFetchHanlder cmtHandler, ImportSpecFileHandler importHandler)
        {
            DataContract contract = new DataContract();
            contract.ContractName = contractName;

            List<DataItem> itemList = new List<DataItem>();
            foreach (string key in itemDict.Keys)
            {
                if (key.StartsWith("!"))
                {
                    if (key.Equals("!Compatibility", StringComparison.InvariantCultureIgnoreCase))
                    {
                        contract.Compatibility = itemDict[key];
                    }
                    else if (key.Equals("!Compatibility-Reference", StringComparison.InvariantCultureIgnoreCase))
                    {
                        contract.Compatibility_Reference = itemDict[key];
                    }
                    else if (key.Equals("!Import", StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (importHandler != null)
                        {
                            string[] defArr = itemDict[key].Trim().Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (string impItem in defArr)
                            {
                                importHandler(srcSpecFile, impItem);
                            }
                        }
                    }
                    else if (key.Equals("!base", StringComparison.InvariantCultureIgnoreCase))
                    {
                        DataContract absDC = srcSpecFile[itemDict[key]];
                        if (absDC != null && absDC.TransItems != null)
                        {
                            itemList.AddRange(absDC.TransItems);
                        }
                    }
                    continue;
                }

                DataItem item = new DataItem();
                item.DataName = key;
                item.NetworkBytes = true;

                #region 设置数据类型和条件传输表达式
                string itemDefine = itemDict[key];
                int idx = itemDefine.IndexOf('{');
                if (idx == -1)
                {
                    item.DataType = itemDefine.Trim();
                }
                else
                {
                    item.DataType = itemDefine.Substring(0, idx).Trim();
                    item.ConditionalExpression = itemDefine.Substring(idx).Trim();
                }
                #endregion

                item.Description = cmtHandler(contractName, key);
                itemList.Add(item);
            }

            contract.ConfigItemCount = itemList.Count;
            contract.TransItems = itemList.ToArray();

            return contract;
        }

        /// <summary>
        /// 加载并解析协议定义文件[TODO]
        /// </summary>
        /// <param name="specUrl">协议定义文件地址</param>
        /// <returns></returns>
        public static SpecFile ParseContractFile(string specUrl, SpecFileFormat format)
        {
            if (format != SpecFileFormat.Ini)
            {
                throw new NotSupportedException();
            }

            List<EnumContract> defList = new List<EnumContract>();
            List<DataContract> impList = new List<DataContract>();
            SpecFile fileDefine = new SpecFile();
            using (IniFile ini = new IniFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, specUrl)))
            {
                fileDefine.FileUrl = ini.Path;
                fileDefine.FileName = Path.GetFileName(ini.Path);

                string[] allSecNames = ini.GetSectionNames();

                Dictionary<string, string> secDict = new Dictionary<string, string>();
                foreach (string secName in allSecNames)
                {
                    #region 对定义区间解析
                    if (secName.Equals("define", StringComparison.InvariantCultureIgnoreCase))
                    {
                        secDict = ini.GetSectionValues(secName);
                        string itemComment = string.Empty;
                        #region 读取枚举定义(define)
                        foreach (var k in secDict.Keys)
                        {
                            itemComment = ini.GetComment(secName, k);
                            defList.Add(EnumContract.Parse(k, itemComment, secDict[k]));
                        }
                        #endregion
                    }
                    else if (secName.Equals("RequestContract", StringComparison.InvariantCultureIgnoreCase))
                    {
                        fileDefine.RequestContract = ReadAsContract(fileDefine, secName, ini.GetSectionValues(secName), (s, k) => ini.GetComment(s, k), ImportSpecFile);
                    }
                    else if (secName.Equals("ResponseContract", StringComparison.InvariantCultureIgnoreCase))
                    {
                        fileDefine.ResponseContract = ReadAsContract(fileDefine, secName, ini.GetSectionValues(secName), (s, k) => ini.GetComment(s, k), ImportSpecFile);
                    }
                    else
                    {
                        #region 读取规范导入(import)
                        impList.Add(ReadAsContract(fileDefine, secName, ini.GetSectionValues(secName), (s, k) => ini.GetComment(s, k), ImportSpecFile));
                        #endregion
                    }
                    #endregion
                }
            }

            fileDefine.MerginDefineWith(defList);
            fileDefine.MerginImportWith(impList);
            return fileDefine;
        }
    }

    /// <summary>
    /// 规范文件格式
    /// </summary>
    public enum SpecFileFormat { Ini, Xml, Json }

    /// <summary>
    /// 导入包含的新的规范文件
    /// </summary>
    /// <param name="srcSpecFile">原始请求规范文件定义</param>
    /// <param name="specDef">规范文件相对位置路径</param>
    public delegate void ImportSpecFileHandler(SpecFile srcSpecFile, string specDef);

    /// <summary>
    /// 导入规范数据
    /// </summary>
    /// <param name="objInstance">规范数据实例</param>
    //public delegate void ImportISpecObject(ISpecObject objInstance);
}

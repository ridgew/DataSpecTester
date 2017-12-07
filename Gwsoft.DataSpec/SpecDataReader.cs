using System;
using System.IO;
using System.Reflection;

#if NUnit
using NUnit.Framework;
#endif

namespace Gwsoft.DataSpec
{
    /// <summary>
    /// 规格数据阅读器
    /// </summary>
#if NUnit
    [TestFixture]
#endif
    public class SpecDataReader
    {

        public SpecDataReader(Stream networkStream)
        {
            bindSteam = networkStream;
        }

        Stream bindSteam = null;


        /// <summary>
        /// 读取全部数据绑定
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns></returns>
        public TEntity ReadToEnd<TEntity>()
            where TEntity : ESPDataBase, new()
        {
            TEntity instance = new TEntity();
            Type instanceType = typeof(TEntity);

            ObjectTransferOrderAttribute[] Configs = new ObjectTransferOrderAttribute[0];
            PropertyInfo[] bindProperties = SpecUtil.GetTransferProperties(instanceType, out Configs);
            object currentObj = null, lastDat = null;

            for (int m = 0, n = bindProperties.Length; m < n; m++)
            {

                string propKey = BindBuilder.GetPropertyBindKey(instanceType, bindProperties[m]);
                if (BindBuilder.CustomBindDict.ContainsKey(propKey))
                {
                    currentObj = BindBuilder.CustomBindDict[propKey](bindSteam, instance);
                    //Console.WriteLine("自定义属性绑定回调..");
                }
                else
                {
                    if (lastDat != null && bindProperties[m].PropertyType.IsArray)
                    {
                        if (Configs[m].ArrayLengthOffset != -1)
                        {
                            throw new NotSupportedException(String.Format("数组属性偏移量不为-1的值暂不支持,当前配置值为:{0}！\r\n实例属性：{1}:{2}, 属性类型：{3}",
                                Configs[m].ArrayLengthOffset,
                                instanceType.FullName,
                                bindProperties[m].Name,
                                bindProperties[m].PropertyType.FullName));
                        }
                        else
                        {
                            currentObj = SpecUtil.GetArrayPropertyValue(bindProperties[m].PropertyType.GetElementType(),
                                Convert.ToInt32(lastDat), bindSteam);
                        }
                    }
                    else
                    {
                        currentObj = SpecUtil.GetNetworkStreamMappingData(bindProperties[m].PropertyType, instance.Context, bindSteam, -1, false);
                    }
                }

                lastDat = currentObj;
                bindProperties[m].SetValue(instance, currentObj, null);
            }

            return instance;
        }

        public int ReadOne<TEntity>(string propertyName)
            where TEntity : ESPDataBase, new()
        {
            ObjectTransferOrderAttribute[] configs = new ObjectTransferOrderAttribute[0];
            PropertyInfo[] totalProperties = SpecUtil.GetTransferProperties(typeof(TEntity), out configs);
            int i = 0, j = totalProperties.Length;
            int targetIndex = 0;
            for (; i < j; i++)
            {
                if (!string.IsNullOrEmpty(propertyName) 
                    && totalProperties[i].Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
                {
                    targetIndex = i;
                    break;
                }
            }

            return j - i - 1;
        }

    }

}

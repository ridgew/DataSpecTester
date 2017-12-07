using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;

namespace Gwsoft.DataSpec
{
    /// <summary>
    /// ESP协议二进制序列化,使用了ObjectTransferOrderAttribute特性标记了的所有公开属性。
    /// </summary>
    public sealed class ESPDataFormatter : IFormatter
    {
        /// <summary>
        /// 初始化 <see cref="ESPDataFormatter"/> class.
        /// </summary>
        public ESPDataFormatter() { }

        /// <summary>
        /// 初始化 <see cref="ESPDataFormatter"/> class.
        /// </summary>
        /// <param name="contextType">Type of the context.</param>
        public ESPDataFormatter(Type contextType)
        {
            Context = new StreamingContext(StreamingContextStates.All, contextType);
        }

        #region IFormatter 成员

        /// <summary>
        /// 获取或设置在反序列化过程中执行类型查找的 <see cref="T:System.Runtime.Serialization.SerializationBinder"/>。
        /// </summary>
        /// <value></value>
        /// <returns>在反序列化过程中执行类型查找的 <see cref="T:System.Runtime.Serialization.SerializationBinder"/>。</returns>
        public SerializationBinder Binder { get; set; }

        /// <summary>
        /// 获取或设置用于序列化和反序列化的 <see cref="T:System.Runtime.Serialization.StreamingContext"/>。
        /// </summary>
        /// <value></value>
        /// <returns>用于序列化和反序列化的 <see cref="T:System.Runtime.Serialization.StreamingContext"/>。</returns>
        public StreamingContext Context { get; set; }

        /// <summary>
        /// 获取或设置当前格式化程序所使用的 <see cref="T:System.Runtime.Serialization.SurrogateSelector"/>。
        /// </summary>
        /// <value></value>
        /// <returns>当前格式化程序所使用的 <see cref="T:System.Runtime.Serialization.SurrogateSelector"/>。</returns>
        public ISurrogateSelector SurrogateSelector { get; set; }

        /// <summary>
        /// 反序列化所提供流中的数据并重新组成对象图形。
        /// </summary>
        /// <param name="serializationStream">包含要反序列化的数据的流。</param>
        /// <returns>反序列化的图形的顶级对象。</returns>
        public object Deserialize(Stream serializationStream)
        {
            Type instanceType = (Type)Context.Context;
            if (!instanceType.IsSubclassOf(typeof(ESPDataBase)))
            {
                throw new InvalidDataException("暂不支持没有基于ESPDataBase对象的反序列化！");
            }

            ESPDataBase instance = null;
            try
            {
                instance = (ESPDataBase)Activator.CreateInstance(instanceType);
                if (serializationStream != null && serializationStream.CanRead)
                    SpecUtil.BindFromNetworkStream(instance, serializationStream, 0, false);
            }
            catch (Exception) { }
            return instance;
        }

        /// <summary>
        /// 将对象或具有给定根的对象图形序列化为所提供的流。
        /// </summary>
        /// <param name="serializationStream">格式化程序在其中放置序列化数据的流。此流可以引用多种后备存储区（如文件、网络、内存等）。</param>
        /// <param name="graph">要序列化的对象或对象图形的根。将自动序列化此根对象的所有子对象。</param>
        public void Serialize(Stream serializationStream, object graph)
        {
            Type InstanceType = graph.GetType();

            ObjectTransferOrderAttribute[] transConfig = new ObjectTransferOrderAttribute[0];
            PropertyInfo[] transPropertys = SpecUtil.GetTransferProperties(InstanceType, out transConfig);

            byte[] currentBytes = new byte[0];
            ObjectTransferOrderAttribute currentConfig = null;
            PropertyInfo pInfo = null;
            for (int i = 0, j = transPropertys.Length; i < j; i++)
            {
                pInfo = transPropertys[i];
                currentConfig = transConfig[i];
                currentBytes = SpecUtil.GetGetHostBytes(pInfo.PropertyType, pInfo.GetValue(graph, null));

                if (currentBytes != null && currentBytes.Length > 0)
                {
                    if (currentConfig.Reverse) currentBytes = SpecUtil.ReverseBytes(currentBytes);
                    serializationStream.Write(currentBytes, 0, currentBytes.Length);
                }

            }
        }

        #endregion
    }
}

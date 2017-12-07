using System;
using System.Collections.Generic;
using System.Text;
using Gwsoft.DataSpec;

#if UnitTest
using NUnit.Framework;
#endif
namespace Gwsoft.EaseMode
{
    /// <summary>
    /// 嵌入资源的文档结构体
    /// </summary>
#if UnitTest
    [TestFixture]
#endif
    public class EmbedResourceDocument : ESPDataBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmbedResourceDocument"/> class.
        /// </summary>
        public EmbedResourceDocument()
            : base()
        { 
        
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmbedResourceDocument"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public EmbedResourceDocument(ESPContext context)
            : base(context)
        { 
        
        }

        /// <summary>
        /// 文档体
        /// </summary>
        [ObjectTransferOrder(0, Reverse = false, Offset = 0)]
        public EaseDocument ESP_Content { get; set; }

        /// <summary>
        /// 文档内资源文件数
        /// </summary>
        [ObjectTransferOrder(1, Reverse = true, Offset = -1)]
        public short ESP_ResourceCount { get; set; }

        /// <summary>
        /// 相关资源数
        /// </summary>
        [ObjectTransferOrder(2, Reverse = false, Offset = 2)]
        public EaseResource[] ESP_Resources { get; set; }


        ///// <summary>
        ///// 自定义属性绑定词典
        ///// </summary>
        //public override void CustomPropertyBindAction()
        //{
        //    BindBuilder.Instance()
        //        .Add((EmbedResourceDocument doc) => doc.ESP_Resources,
        //        (s, obj) => {
        //            EmbedResourceDocument cRDoc = (EmbedResourceDocument)obj;
        //            cRDoc.ESP_Resources = s.GetCurrentContainerEntities<EaseResource, EmbedResourceDocument>(cRDoc, r => (int)r.ESP_ResourceCount);
        //            return cRDoc.ESP_Resources;
        //        })
        //        .End<EmbedResourceDocument>();
        //}


#if UnitTest
        private static EmbedResourceDocument _embedResourceDocument = null;

        public static EmbedResourceDocument EmbedResourceDocument4Test
        {
            get {
                if (_embedResourceDocument == null)
                {
                    EmbedResourceDocument instance = new EmbedResourceDocument();
                    instance.ESP_Content = EaseDocument.EaseDocument4Test;

                    instance.ESP_ResourceCount = 1;
                    instance.ESP_Resources = new EaseResource[] { EaseResource.EaseResource4Test };

                    _embedResourceDocument = instance;
                }
                return _embedResourceDocument;
            }
        }

        [Test]
        public void DoTest()
        {

            EmbedResourceDocument instance = EmbedResourceDocument4Test;

            byte[] testBytes = instance.GetNetworkBytes();
            //Console.WriteLine("Total:{0}\r\n{1}", testBytes.ESP_Length, testBytes.GetHexViewString());

            EmbedResourceDocument instanceCmp = new EmbedResourceDocument();
            System.IO.MemoryStream ms = new System.IO.MemoryStream(testBytes);
            ms.Position = 0;
            instanceCmp.BindFromNetworkStream(ms, 0, false);

            byte[] bytes2cmp = instanceCmp.GetNetworkBytes();
            //Console.WriteLine("Cmp Total:{0}\r\n{1}", bytes2cmp.ESP_Length, bytes2cmp.GetHexViewString());

            Assert.That(SpecUtil.AreEqual(testBytes, bytes2cmp));

        }

#endif

    }
}

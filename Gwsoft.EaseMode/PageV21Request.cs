using System;
using System.Collections.Generic;
using System.Text;
using Gwsoft.Configuration;
using Gwsoft.DataSpec;

#if UnitTest
using NUnit.Framework;
#endif

namespace Gwsoft.EaseMode
{
    /// <summary>
    /// 页面请求－兼容2.1
    /// </summary>
    [ImplementState(CompleteState.OK, "1.0(v3.2-2.6.3.1)", Description = "网络接入页面请求－兼容2.1", ReleaseDateGTM = "Mon, 04 Jan 2010 16:28:16 GMT")]
#if UnitTest
    [TestFixture]
#endif
    public class PageV21Request : RequestBase
#if UnitTest
     , ISelfUnitTest
#endif
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PageV21Request"/> class.
        /// </summary>
        public PageV21Request()
            : base()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PageV21Request"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public PageV21Request(ESPContext context)
            : base(context)
        { }

        #region 继承属性模型顺序
        /// <summary>
        /// 页面链接
        /// </summary>
        [ObjectTransferOrder(20, Reverse = false, Offset = -1)]
        public EaseString ESP_Link { get; set; }
        #endregion

#if UnitTest
        [Test]
        public void DoRequest()
        {
            byte[] linkBytes = EaseString.DefaultEncoding.GetBytes("http://118.123.205.218:888/blbook/index.aspx");

            PageV21Request instance = new PageV21Request
            {
                ESP_Link = new EaseString { ESP_Length = (short)linkBytes.Length, ESP_Data = linkBytes },
                ESP_Header = RequestHeader.RequestHeader4Test
            };

            //ServiceConfig Config = ServiceConfig.GetByServiceID(1);

            byte[] testBytes = instance.GetNetworkBytes();
            //Console.WriteLine("Total:{0}\r\n{1}", testBytes.ESP_Length, testBytes.GetHexViewString());

            PageV21Request instanceCmp = new PageV21Request();
            System.IO.MemoryStream ms = new System.IO.MemoryStream(testBytes);
            ms.Position = 0;
            instanceCmp.BindFromNetworkStream(ms, 0, false);
            byte[] bytes2cmp = instanceCmp.GetNetworkBytes();
            //Console.WriteLine("Cmp Total:{0}\r\n{1}", bytes2cmp.ESP_Length, bytes2cmp.GetHexViewString());

            Assert.That(SpecUtil.AreEqual(testBytes, bytes2cmp));
            
        }


        #region ISelfUnitTest 成员

        public TestResult SelfTest(TestContext context)
        {
            return TestContext.Run(context, DoRequest);
        }

        #endregion
#endif

    }


    /// <summary>
    /// 页面响应－兼容2.1
    /// </summary>
    [ImplementState(CompleteState.OK, "1.0(v3.1-2.6.3.2)", Description = "网络接入页面响应－兼容2.1", ReleaseDateGTM = "Mon, 04 Jan 2010 16:53:12 GMT")]
#if UnitTest
    [TestFixture]
#endif
    public class PageV21Response : ResponseBase
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="PageV21Response"/> class.
        /// </summary>
        public PageV21Response()
            : base()
        { 
        
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PageV21Response"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public PageV21Response(ESPContext context)
            : base(context)
        { 
        
        }

        #region 继承属性模型顺序
        /// <summary>
        /// 页面文档数
        /// </summary>
        [ObjectTransferOrder(20, Reverse = true, Offset = -1)]
        public short ESP_PageDocCount { get; set; }

        /// <summary>
        /// 页面文档数的内容（长度为PageDocCount）
        /// </summary>
        [ObjectTransferOrder(21, Reverse = false, Offset = 2)]
        public EmbedResourceDocument[] ESP_EmbedResDocs { get; set; }
        #endregion

        ///// <summary>
        ///// 自定义属性绑定词典
        ///// </summary>
        //public override void CustomPropertyBindAction()
        //{
        //    BindBuilder.Instance()
        //        .Add((PageV21Response resp) => resp.ESP_EmbedResDocs, //文档对象绑定
        //         (s, obj) =>
        //         {
        //             PageV21Response cResp = (PageV21Response)obj;
        //             cResp.ESP_EmbedResDocs = s.GetCurrentContainerEntities<EmbedResourceDocument, PageV21Response>(cResp, r => (int)r.ESP_PageDocCount);
        //             return cResp.ESP_EmbedResDocs;
        //         })
        //         .End<PageV21Response>();
        //}

#if UnitTest

        [Test]
        public void DoTest()
        {
            PageV21Response instance = new PageV21Response();

            instance.ESP_Header = ResponseHeader.ResponseHeader4Test;
            instance.ESP_Code = StatusCode.Success;
            byte[] msgBytes = EaseString.DefaultEncoding.GetBytes("OK");
            instance.ESP_Message = new EaseString { ESP_Data = msgBytes, ESP_Length = (short)msgBytes.Length };
            instance.ESP_Method = CommandType.None;

            byte[] cmdBytes = EaseString.DefaultEncoding.GetBytes("GET / HTTP/1.1");
            instance.ESP_Command = new EaseString { ESP_Data = cmdBytes, ESP_Length = (short)cmdBytes.Length };
            instance.ESP_PageDocCount = 1;

            instance.ESP_EmbedResDocs = new EmbedResourceDocument[] { EmbedResourceDocument.EmbedResourceDocument4Test };


            byte[] testBytes = instance.GetNetworkBytes();
            //Console.WriteLine("Total:{0}\r\n{1}", testBytes.ESP_Length, testBytes.GetHexViewString());

            PageV21Response instanceCmp = new PageV21Response();
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

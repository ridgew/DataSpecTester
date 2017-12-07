using System;
using System.Collections.Generic;
using System.Text;
using Gwsoft.Configuration;
using Gwsoft.DataSpec;
using System.IO;
#if UnitTest
using NUnit.Framework;
#endif
namespace Gwsoft.EaseMode
{
    /// <summary>
    /// 页面文档请求
    /// </summary>
    [ImplementState(CompleteState.OK, "1.0(v3.1-2.6.5.1)", Description = "页面文档请求", ReleaseDateGTM = "Mon, 04 Jan 2010 12:37:06 GMT")]
#if UnitTest
    [TestFixture]
#endif
    public class PageRequest : RequestBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PageRequest"/> class.
        /// </summary>
        public PageRequest()
            : base()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PageRequest"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public PageRequest(ESPContext context)
            : base(context)
        { }

        #region 属性模型顺序
        /// <summary>
        /// 页面链接
        /// </summary>
        [ObjectTransferOrder(3, Reverse = false, Offset = -1)]
        public EaseString ESP_Link { get; set; }
        #endregion

#if UnitTest
        [Test]
        public void DoTest()
        {
            PageRequest instance = new PageRequest();
            instance.ESP_Header = RequestHeader.RequestHeader4Test;

            byte[] linkBytes = Encoding.UTF8.GetBytes("http://118.123.205.218:888/blbook/index.aspx");
            instance.ESP_Link = new EaseString { ESP_Data = linkBytes, ESP_Length = (short)linkBytes.Length };

            byte[] testBytes = instance.GetNetworkBytes();
            //Console.WriteLine("连接长度：{0}, Hex:{1}", linkBytes.ESP_Length, BitConverter.ToString(linkBytes));
            //Console.WriteLine("Total:{0}\r\n{1}", testBytes.ESP_Length, testBytes.GetHexViewString());

            PageRequest instanceCmp = new PageRequest();
            System.IO.MemoryStream ms = new System.IO.MemoryStream(testBytes);
            ms.Position = 0;
            instanceCmp.BindFromNetworkStream(ms, 0, false);

            //byte[] dumpBytes = ms.ToArray();
            //Console.WriteLine("Dump Total:{0}\r\n{1}", dumpBytes.ESP_Length, dumpBytes.GetHexViewString());

            //Console.WriteLine("OLD ESP_Method: {0} == New : {1}", instance.ESP_Header.ESP_Method, instanceCmp.ESP_Header.ESP_Method);
            //Console.WriteLine("OLD ESP_Link Range: {0}-{1} == New : {2}-{3}",
            //    instance.ESP_Link.ContentRange[0],
            //    instance.ESP_Link.ContentRange[1],
            //    instanceCmp.ESP_Link.ContentRange[0],
            //    instanceCmp.ESP_Link.ContentRange[1]);

            byte[] bytes2cmp = instanceCmp.GetNetworkBytes();
            //Console.WriteLine("Cmp Total:{0}\r\n{1}", bytes2cmp.ESP_Length, bytes2cmp.GetHexViewString());

            Assert.That(SpecUtil.AreEqual(testBytes, bytes2cmp));

        }
#endif

    }


    /// <summary>
    /// 页面文档响应
    /// </summary>
    [ImplementState(CompleteState.OK, "1.0(v3.1-2.6.5.2)", Description = "页面文档响应", ReleaseDateGTM = "Mon, 04 Jan 2010 15:58:26 GMT")]
#if UnitTest
    [TestFixture]
#endif
    public class PageResponse : ResponseBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PageResponse"/> class.
        /// </summary>
        public PageResponse()
            : base()
        {
            //internalImplementDataBind = true;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="PageResponse"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public PageResponse(ESPContext context)
            : base(context)
        {
            //internalImplementDataBind = true;
        }

        #region 继承属性模型顺序
        /// <summary>
        /// 页面文档数
        /// </summary>
        [ObjectTransferOrder(20, Reverse = true, Offset = -1)]
        public short ESP_PageDocCount { get; set; }

        /// <summary>
        /// 页面文档内容
        /// </summary>
        [ObjectTransferOrder(21, Reverse = false, Offset = 2)]
        public EaseDocument[] ESP_Docs { get; set; }
        #endregion



        ///// <summary>
        ///// 自定义属性绑定词典
        ///// </summary>
        //public override void CustomPropertyBindAction()
        //{
        //    BindBuilder.Instance()
        //        .Add((PageResponse resp) => resp.ESP_Docs, //文档对象绑定
        //         (s, obj) =>
        //         {
        //             PageResponse cResp = (PageResponse)obj;
        //             cResp.ESP_Docs = s.GetCurrentContainerEntities<EaseDocument, PageResponse>(cResp, r => (int)r.ESP_PageDocCount);
        //             return cResp.ESP_Docs;
        //         })
        //         .End<PageResponse>();
        //}


#if UnitTest
        [Test]
        public void DoTest()
        {
            PageResponse instance = new PageResponse();
            instance.ESP_Header = ResponseHeader.ResponseHeader4Test;
            instance.ESP_Code = StatusCode.Success;

            byte[] msgBytes = EaseString.DefaultEncoding.GetBytes("OK");
            instance.ESP_Message = new EaseString { ESP_Data = msgBytes, ESP_Length = (short)msgBytes.Length };

            instance.ESP_Method = CommandType.None;

            byte[] cmdBytes = EaseString.DefaultEncoding.GetBytes("GET / HTTP/1.1");
            instance.ESP_Command = new EaseString { ESP_Data = cmdBytes, ESP_Length = (short)cmdBytes.Length };

            instance.ESP_PageDocCount = 1;
            instance.ESP_Docs = new EaseDocument[] { EaseDocument.EaseDocument4Test };


            byte[] testBytes = instance.GetNetworkBytes();
            //Console.WriteLine("Total:{0}\r\n{1}", testBytes.ESP_Length, testBytes.GetHexViewString());

            PageResponse instanceCmp = new PageResponse();
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

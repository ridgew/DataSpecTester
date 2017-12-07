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
    /// 资源请求
    /// </summary>
    [ImplementState(CompleteState.OK, "1.0(v3.2-2.6.6.1)", Description = "资源请求", ReleaseDateGTM = "Tue, 05 Jan 2010 09:57:37 GMT")]
#if UnitTest
    [TestFixture]
#endif
    public class ResourceRequest : RequestBase
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceRequest"/> class.
        /// </summary>
        public ResourceRequest()
            : base()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceRequest"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public ResourceRequest(ESPContext context)
            : base(context)
        { }


        #region 继承属性模型顺序
        /// <summary>
        /// 请求的包序号(为0不分包，分包首个包序号为1)
        /// </summary>
        [ObjectTransferOrder(20, Reverse = true, Offset = -1)]
        public short ESP_PackageIndex { get; set; }

        /// <summary>
        /// 请求的包长度(为0不分包)
        /// </summary>
        [ObjectTransferOrder(21, Reverse = true, Offset = 2)]
        public int ESP_PackageLength { get; set; }

        /// <summary>
        /// 请求资源链接数
        /// </summary>
        [ObjectTransferOrder(22, Reverse = true, Offset = 4)]
        public short ESP_LinksCount { get; set; }

        /// <summary>
        /// 资源链接数据
        /// </summary>
        [ObjectTransferOrder(23, Reverse = false, Offset = 2)]
        public EaseString[] ESP_LinkData { get; set; }
        #endregion

        ///// <summary>
        ///// 自定义属性绑定词典
        ///// </summary>
        //public override void CustomPropertyBindAction()
        //{
        //    BindBuilder.Instance()
        //        .Add((ResourceRequest req) => req.ESP_LinkData,
        //            (s, obj) =>
        //            {
        //                ResourceRequest cReq = (ResourceRequest)obj;
        //                cReq.ESP_LinkData = SpecUtil.GetCurrentContainerEntities<EaseString, ResourceRequest>(s, cReq, r => (int)r.ESP_LinksCount);
        //                return cReq.ESP_LinkData;
        //            })
        //        .End<ResourceRequest>();
        //}

#if UnitTest


        [Test]
        public void DoTest()
        {
            ResourceRequest instance = new ResourceRequest();
            instance.ESP_Header = RequestHeader.RequestHeader4Test;
            instance.ESP_PackageIndex = 0;
            instance.ESP_PackageLength = 0;
            instance.ESP_LinksCount = 1;

            byte[] linkDat = EaseString.DefaultEncoding.GetBytes("http://118.123.205.218:888/images/testpic.gif");
            instance.ESP_LinkData = new EaseString[] { 
                new EaseString { ESP_Data = linkDat, ESP_Length=(short)linkDat.Length }
            };


            byte[] testBytes = instance.GetNetworkBytes();
            //Console.WriteLine("Total:{0}\r\n{1}", testBytes.ESP_Length, testBytes.GetHexViewString());

            ResourceRequest instanceCmp = new ResourceRequest();
            System.IO.MemoryStream ms = new System.IO.MemoryStream(testBytes);
            ms.Position = 0;
            instanceCmp.BindFromNetworkStream(ms, 0, false);

            byte[] bytes2cmp = instanceCmp.GetNetworkBytes();
            //Console.WriteLine("Cmp Total:{0}\r\n{1}", bytes2cmp.ESP_Length, bytes2cmp.GetHexViewString());

            Assert.That(SpecUtil.AreEqual(testBytes, bytes2cmp));

        }
#endif

    }

    /// <summary>
    /// 资源响应
    /// </summary>
    [ImplementState(CompleteState.OK, "1.0(v3.2-2.6.6.2)", Description = "资源响应", ReleaseDateGTM = "Tue, 05 Jan 2010 10:37:12 GMT")]
#if UnitTest
    [TestFixture]
#endif
    public class ResourceResponse : ResponseBase
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceResponse"/> class.
        /// </summary>
        public ResourceResponse()
            : base()
        { }


        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceResponse"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public ResourceResponse(ESPContext context)
            : base(context)
        { }

        #region 继承属性模型顺序
        /// <summary>
        /// 资源文件数
        /// </summary>
        [ObjectTransferOrder(20, Reverse = true, Offset = -1)]
        public short ESP_PageResCount { get; set; }

        /// <summary>
        /// 资源文件内容
        /// </summary>
        [ObjectTransferOrder(21, Reverse = false, Offset = 2)]
        public EaseResource[] ESP_Resources { get; set; }
        #endregion

        ///// <summary>
        ///// 自定义属性绑定词典
        ///// </summary>
        //public override void CustomPropertyBindAction()
        //{
        //    BindBuilder.Instance()
        //         .Add((ResourceResponse resp) => resp.ESP_Resources,
        //             (s, obj) =>
        //             {
        //                 ResourceResponse cResp = (ResourceResponse)obj;
        //                 cResp.ESP_Resources = SpecUtil.GetCurrentContainerEntities<EaseResource, ResourceResponse>(s, cResp, r => (int)r.ESP_PageResCount);
        //                 return cResp.ESP_Resources;
        //             })
        //         .End<ResourceResponse>();
        //}

#if UnitTest


        [Test]
        public void DoTest()
        {
            ResourceResponse instance = new ResourceResponse();
            instance.ESP_Header = ResponseHeader.ResponseHeader4Test;
            instance.ESP_Code = StatusCode.Success;
            byte[] msgBytes = EaseString.DefaultEncoding.GetBytes("OK");
            instance.ESP_Message = new EaseString { ESP_Data = msgBytes, ESP_Length = (short)msgBytes.Length };
            instance.ESP_Method = CommandType.None;
            byte[] cmdBytes = EaseString.DefaultEncoding.GetBytes("GET / HTTP/1.1");
            instance.ESP_Command = new EaseString { ESP_Data = cmdBytes, ESP_Length = (short)cmdBytes.Length };

            instance.ESP_PageResCount = 1;
            instance.ESP_Resources = new EaseResource[] { 
                EaseResource.EaseResource4Test
            };


            byte[] testBytes = instance.GetNetworkBytes();
            //Console.WriteLine("Total:{0}\r\n{1}", testBytes.ESP_Length, testBytes.GetHexViewString());

            ResourceResponse instanceCmp = new ResourceResponse();
            System.IO.MemoryStream ms = new System.IO.MemoryStream(testBytes);
            ms.Position = 0;
            instanceCmp.BindFromNetworkStream(ms, 0, false);

            byte[] bytes2cmp = instanceCmp.GetNetworkBytes();
            //Console.WriteLine("Cmp Total:{0}\r\n{1}", bytes2cmp.ESP_Length, bytes2cmp.GetHexViewString());

            Assert.That(SpecUtil.AreEqual(testBytes, bytes2cmp));

        }
#endif

    }


    /// <summary>
    /// 资源分包响应
    /// </summary>
    [ImplementState(CompleteState.OK, "1.0(v3.2-2.6.6.3)", Description = "资源分包响应", ReleaseDateGTM = "Tue, 05 Jan 2010 11:27:02 GMT")]
#if UnitTest
    [TestFixture]
#endif
    public class ResourcePartialResponse : PackageResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourcePartialResponse"/> class.
        /// </summary>
        public ResourcePartialResponse()
            : base()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourcePartialResponse"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public ResourcePartialResponse(ESPContext context)
            : base(context)
        { }

        ///// <summary>
        ///// 自定义属性绑定词典
        ///// </summary>
        //public override void CustomPropertyBindAction()
        //{
        //    SubClassPropertyBindAction<ResourcePartialResponse>()
        //        .End<ResourcePartialResponse>();
        //}


#if UnitTest


        [Test]
        public void DoTest()
        {
            ResourcePartialResponse instance = new ResourcePartialResponse();

            instance.ESP_Header = ResponseHeader.ResponseHeader4Test;
            instance.ESP_Code = StatusCode.Success;
            byte[] msgBytes = EaseString.DefaultEncoding.GetBytes("OK");
            instance.ESP_Message = new EaseString { ESP_Data = msgBytes, ESP_Length = (short)msgBytes.Length };

            instance.ESP_PackageIndex = 1;
            instance.ESP_LeavePackageCount = 0;

            instance.ESP_PackageData = Encoding.ASCII.GetBytes("12345678");
            instance.ESP_PackageLength = instance.ESP_PackageData.Length;
            

            byte[] testBytes = instance.GetNetworkBytes();
            //Console.WriteLine("Total:{0}\r\n{1}", testBytes.ESP_Length, testBytes.GetHexViewString());

            ResourcePartialResponse instanceCmp = new ResourcePartialResponse();
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

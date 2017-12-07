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
    /// 应用功能请求
    /// </summary>
    [ImplementState(CompleteState.OK, "1.0(v3.2-2.6.7.1)", Description = "应用功能请求", ReleaseDateGTM = "Tue, 05 Jan 2010 10:47:49 GMT")]
#if UnitTest
    [TestFixture]
#endif
    public class ApplicationRequest : RequestBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationRequest"/> class.
        /// </summary>
        public ApplicationRequest()
            : base()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationRequest"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public ApplicationRequest(ESPContext context)
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
        /// 应用服务器ID, 通常情况下和头信息中业务代码保持一致，当调用第三方应用时可能不同。
        /// </summary>
        [ObjectTransferOrder(22, Reverse = true, Offset = 4)]
        public int ESP_AppServerID { get; set; }

        /// <summary>
        /// 应用请求长度
        /// </summary>
        [ObjectTransferOrder(23, Reverse = true, Offset = 4)]
        public int ESP_AppRequestLength { get; set; }

        /// <summary>
        /// 应用请求数据
        /// </summary>
        [ObjectTransferOrder(24, Reverse = false, Offset = 4)]
        public byte[] ESP_AppRequestData { get; set; }
        #endregion

        ///// <summary>
        ///// 自定义属性绑定词典
        ///// </summary>
        //public override void CustomPropertyBindAction()
        //{
        //    BindBuilder.Instance()
        //        .Add((ApplicationRequest req) => req.ESP_AppRequestData,
        //            (s, obj) =>
        //            {
        //                ApplicationRequest cReq = (ApplicationRequest)obj;
        //                cReq.ESP_AppRequestData = SpecUtil.ReadNetworkStreamBytes(s, cReq.ESP_AppRequestLength);
        //                return cReq.ESP_AppRequestData;
        //            })
        //        .End<ApplicationRequest>();
        //}

#if UnitTest


        [Test]
        public void DoTest()
        {
            ApplicationRequest instance = new ApplicationRequest();
            instance.ESP_Header = RequestHeader.RequestHeader4Test;
            instance.ESP_PackageIndex = 0;
            instance.ESP_PackageLength = 0;
            instance.ESP_AppServerID = 1;

            byte[] linkDat = EaseString.DefaultEncoding.GetBytes("http://118.123.205.218:888/images/testpic.gif");
            instance.ESP_AppRequestData = linkDat;
            instance.ESP_AppRequestLength = linkDat.Length;


            byte[] testBytes = instance.GetNetworkBytes();

            Assert.That(ESPDataBase.IsValidNetworkBytes<ApplicationRequest>(testBytes, Console.Out));
        }
#endif

    }


    /// <summary>
    /// 应用功能响应
    /// </summary>
    [ImplementState(CompleteState.OK, "1.0(v3.2-2.6.7.2)", Description = "应用功能响应", ReleaseDateGTM = "Tue, 05 Jan 2010 13:47:49 GMT")]
#if UnitTest
    [TestFixture]
#endif
    public class ApplicationResponse : ResponseBase
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationResponse"/> class.
        /// </summary>
        public ApplicationResponse()
            : base()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationResponse"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public ApplicationResponse(ESPContext context)
            : base(context)
        { }


        #region 继承属性模型顺序
        /// <summary>
        /// 应用响应数据长度
        /// </summary>
        [ObjectTransferOrder(20, Reverse = true, Offset = -1)]
        public int ESP_AppResponseLength { get; set; }

        /// <summary>
        /// 应用响应数据
        /// </summary>
        [ObjectTransferOrder(21, Reverse = false, Offset = 4)]
        public byte[] ESP_AppResponseData { get; set; }
        #endregion

        ///// <summary>
        ///// 填充绑定词典
        ///// </summary>
        //public override void CustomPropertyBindAction()
        //{
        //    BindBuilder.Instance()
        //         .Add((ApplicationResponse resp) => resp.ESP_AppResponseData,
        //             (s, obj) =>
        //             {
        //                 ApplicationResponse cResp = (ApplicationResponse)obj;
        //                 cResp.ESP_AppResponseData = SpecUtil.ReadNetworkStreamBytes(s, cResp.ESP_AppResponseLength);
        //                 return cResp.ESP_AppResponseData;
        //             })
        //         .End<ApplicationResponse>();
        //}

#if UnitTest


        [Test]
        public void DoTest()
        {
            ApplicationResponse instance = new ApplicationResponse();
            instance.ESP_Header = ResponseHeader.ResponseHeader4Test;
            instance.ESP_Code = StatusCode.Success;
            byte[] msgBytes = EaseString.DefaultEncoding.GetBytes("OK");
            instance.ESP_Message = new EaseString { ESP_Data = msgBytes, ESP_Length = (short)msgBytes.Length };
            instance.ESP_Method = CommandType.None;
            byte[] cmdBytes = EaseString.DefaultEncoding.GetBytes("GET / HTTP/1.1");
            instance.ESP_Command = new EaseString { ESP_Data = cmdBytes, ESP_Length = (short)cmdBytes.Length };

            byte[] linkDat = EaseString.DefaultEncoding.GetBytes("http://118.123.205.218:888/images/testpic.gif");
            instance.ESP_AppResponseData = linkDat;
            instance.ESP_AppResponseLength = linkDat.Length;


            byte[] testBytes = instance.GetNetworkBytes();
            //Console.WriteLine("Total:{0}\r\n{1}", testBytes.ESP_Length, testBytes.GetHexViewString());

            ApplicationResponse instanceCmp = new ApplicationResponse();
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
    /// 应用功能分包响应
    /// </summary>
    [ImplementState(CompleteState.OK, "1.0(v3.2-2.6.7.3)", Description = "应用功能分包响应", ReleaseDateGTM = "Tue, 05 Jan 2010 13:47:49 GMT")]
    public class ApplicationPartialResponse : PackageResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationPartialResponse"/> class.
        /// </summary>
        public ApplicationPartialResponse()
            : base()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationPartialResponse"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public ApplicationPartialResponse(ESPContext context)
            : base(context)
        { }

        ///// <summary>
        ///// 自定义属性绑定词典
        ///// </summary>
        //public override void CustomPropertyBindAction()
        //{
        //    SubClassPropertyBindAction<ApplicationPartialResponse>()
        //        .End<ApplicationPartialResponse>();
        //}
    }
}

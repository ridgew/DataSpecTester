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
    /// 更新服务器访问地址(请求封装)
    /// </summary>
    [ImplementState(CompleteState.OK, "1.0(v3.2-2.6.8.1)", Description = "更新服务器访问地址(请求封装)", ReleaseDateGTM = "Tue, 05 Jan 2010 05:57:56 GMT")]
    public class GatewayUpdateRequest : RequestBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GatewayUpdateRequest"/> class.
        /// </summary>
        public GatewayUpdateRequest()
            : base()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="GatewayUpdateRequest"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public GatewayUpdateRequest(ESPContext context)
            : base(context)
        { }


    }

    /// <summary>
    /// 更新服务器访问地址(响应封装)
    /// </summary>
    [ImplementState(CompleteState.OK, "1.0(v3.2-2.6.8.1)", Description = "更新服务器访问地址(响应封装)", ReleaseDateGTM = "Tue, 05 Jan 2010 05:57:56 GMT")]
#if UnitTest
    [TestFixture]
#endif
    public class GatewayUpdateResponse : ResponseBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GatewayUpdateResponse"/> class.
        /// </summary>
        public GatewayUpdateResponse()
            : base()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="GatewayUpdateResponse"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public GatewayUpdateResponse(ESPContext context)
            : base(context)
        { }

        #region 继承属性模型顺序

        /// <summary>
        /// Socket 方式连接地址(IP)
        /// <para>格式示例: 10.199.112.14:9010</para>
        /// </summary>
        [ObjectTransferOrder(20, Reverse = false, Offset = -1)]
        public EaseString ESP_SocketAddress { get; set; }

        /// <summary>
        /// Http方式连接地址
        /// <para>格式示例:http://www.easegateway.com:7001/ease/servlet/ease</para>
        /// </summary>
        [ObjectTransferOrder(21, Reverse = false, Offset = -1)]
        public EaseString ESP_HttpAddress { get; set; }

        #endregion

#if UnitTest

        [Test]
        public void DoTest()
        {
            GatewayUpdateResponse instance = new GatewayUpdateResponse();
            instance.ESP_Header = ResponseHeader.ResponseHeader4Test;
            instance.ESP_Code = StatusCode.Success;
            byte[] msgBytes = EaseString.DefaultEncoding.GetBytes("OK");
            instance.ESP_Message = new EaseString { ESP_Data = msgBytes, ESP_Length = (short)msgBytes.Length };
            instance.ESP_Method = CommandType.None;
            byte[] cmdBytes = EaseString.DefaultEncoding.GetBytes("GET / HTTP/1.1");
            instance.ESP_Command = new EaseString { ESP_Data = cmdBytes, ESP_Length = (short)cmdBytes.Length };

            byte[] linkDat = EaseString.DefaultEncoding.GetBytes("http://www.easegateway.com:7001/ease/servlet/ease");
            instance.ESP_HttpAddress = new EaseString { ESP_Data = linkDat, ESP_Length = (short)linkDat.Length };

            byte[] socketDat = EaseString.DefaultEncoding.GetBytes("10.199.112.14:9010");
            instance.ESP_SocketAddress = new EaseString { ESP_Data = socketDat, ESP_Length = (short)socketDat.Length };


            byte[] testBytes = instance.GetNetworkBytes();
            //Console.WriteLine("Total:{0}\r\n{1}", testBytes.ESP_Length, testBytes.GetHexViewString());

            GatewayUpdateResponse instanceCmp = new GatewayUpdateResponse();
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

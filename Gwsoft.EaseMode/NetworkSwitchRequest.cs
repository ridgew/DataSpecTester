using System;
using System.Collections.Generic;
using System.Text;
using Gwsoft.Configuration;
using Gwsoft.DataSpec;
using System.Reflection;
using System.IO;

#if UnitTest
using NUnit.Framework;
#endif
namespace Gwsoft.EaseMode
{
    /// <summary>
    /// 2.5	EASE网络接入网关协议定义
    /// </summary>
#if UnitTest
    [TestFixture]
#endif
    [ImplementState(CompleteState.OK, "1.0(v3.2-2.5.1)", Description = "EASE网络接入网关请求", ReleaseDateGTM = "Wed, 30 Dec 2009 17:09:16 GMT")]
    public sealed class NetworkSwitchRequest : ESPDataBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkSwitchRequest"/> class.
        /// </summary>
        public NetworkSwitchRequest()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkSwitchRequest"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public NetworkSwitchRequest(ESPContext context)
            : base(context)
        {
        }

        /// <summary>
        /// 填充绑定词典
        /// </summary>
        public override void CustomPropertyBindAction()
        {
            BindBuilder.Instance()
                .Add((NetworkSwitchRequest req) => req.ESP_SocketParamCount, //SOCKET头参数个数(7)
                 (s, l, obj) =>
                 {
                     NetworkSwitchRequest cRequest = (NetworkSwitchRequest)obj;
                     if (cRequest.ESP_SuccessFlag == EaseSuccessFlag.Success)
                     {
                         cRequest.ESP_SocketParamCount = s.ReadNetworkStreamAsEntity<short>(2);
                     }
					 return new PropertyBindState { PropertyValue = cRequest.ESP_SocketParamCount, StreamBind = true };
                 })
                 .Add((NetworkSwitchRequest req) => req.ESP_EncodeType,   //客户端编码(9)
				 (s, l, obj) =>
                 {
                     NetworkSwitchRequest cRequest = (NetworkSwitchRequest)obj;
                     if (cRequest.ESP_SuccessFlag == EaseSuccessFlag.Success)
                     {
                         cRequest.ESP_EncodeType = s.ReadNetworkStreamAsEntity<EaseEncode>(1);
                     }
                     return new PropertyBindState { PropertyValue = cRequest.ESP_EncodeType, StreamBind = true };
                 })
                 .Add((NetworkSwitchRequest req) => req.ESP_DataIndex,   //返回数据起始位置(11)
				 (s, l, obj) =>
                 {
                     NetworkSwitchRequest cRequest = (NetworkSwitchRequest)obj;
                     if (cRequest.ESP_SuccessFlag == EaseSuccessFlag.Success)
                     {
                         cRequest.ESP_DataIndex = s.ReadNetworkStreamAsEntity<int>(4);
                     }
                     return new PropertyBindState { PropertyValue = cRequest.ESP_DataIndex, StreamBind = true };
                 })
                 .Add((NetworkSwitchRequest req) => req.ESP_DateEndIndex,   //数据终止位置(13)
				 (s, l, obj) =>
                 {
                     NetworkSwitchRequest cRequest = (NetworkSwitchRequest)obj;
                     if (cRequest.ESP_SuccessFlag == EaseSuccessFlag.Success)
                     {
                         cRequest.ESP_DateEndIndex = s.ReadNetworkStreamAsEntity<int>(4);
                     }
                     return new PropertyBindState { PropertyValue = cRequest.ESP_DateEndIndex, StreamBind = true };
                 })
                 .Add((NetworkSwitchRequest req) => req.ESP_AppServerID,   //应用服务器的地址ID(15)
				 (s, l, obj) =>
                 {
                     NetworkSwitchRequest cRequest = (NetworkSwitchRequest)obj;
                     if (cRequest.ESP_SuccessFlag == EaseSuccessFlag.Success)
                     {
                         cRequest.ESP_AppServerID = s.ReadNetworkStreamAsEntity<short>(2);
                     }
                     return new PropertyBindState { PropertyValue = cRequest.ESP_AppServerID, StreamBind = true };
                 })
                 .Add((NetworkSwitchRequest req) => req.ESP_Link,   //请求链接(17)
				 (s, l, obj) =>
                 {
                     NetworkSwitchRequest cRequest = (NetworkSwitchRequest)obj;
                     //45
                     if (cRequest.ESP_SuccessFlag == EaseSuccessFlag.Success)
                     {
                         cRequest.ESP_Link = s.DataBind<EaseString>(l);
                     }
                     return new PropertyBindState { PropertyValue = cRequest.ESP_Link, StreamBind = true };
                 })
                 .Add((NetworkSwitchRequest req) => req.ESP_TransferLength,   //应用请求数据(19)
				 (s, l, obj) =>
                 {
                     NetworkSwitchRequest cRequest = (NetworkSwitchRequest)obj;
                     if (cRequest.ESP_SuccessFlag == EaseSuccessFlag.Success)
                     {
                         cRequest.ESP_TransferLength = s.ReadNetworkStreamAsEntity<int>(4);
                     }
                     return new PropertyBindState { PropertyValue = cRequest.ESP_TransferLength, StreamBind = true };
                 })
                 .Add((NetworkSwitchRequest req) => req.ESP_TransferData,   /*ESP_TransferData*/
				 (s, l, obj) =>
                 {
                     NetworkSwitchRequest cRequest = (NetworkSwitchRequest)obj;
                     int readLen = (cRequest.ESP_SuccessFlag == EaseSuccessFlag.Success) ? cRequest.ESP_TransferLength : cRequest.ESP_LeaveLength;
                     cRequest.ESP_TransferData = s.ReadNetworkStreamBytes(readLen, true);
					 return new PropertyBindState { PropertyValue = cRequest.ESP_TransferData, StreamBind = true };
                 })
                 .End<NetworkSwitchRequest>();
        }

        #region 属性模型顺序
        /// <summary>
        /// 网络连接成功标志(short)
        /// </summary>
        [ObjectTransferOrder(0, Reverse = true, Offset = 0)]
        public EaseSuccessFlag ESP_SuccessFlag { get; set; }

        /// <summary>
        /// 客户端自定义包序号（0 – 心跳包 ，其它值为客户端自定义序号）
        /// </summary>
        [ObjectTransferOrder(3, Reverse = true, Offset = 2)]
        public int ESP_CustomeCode { get; set; }

        /// <summary>
        /// 请求包后续长度(不包含此参数长度)
        /// </summary>
        [ObjectTransferOrder(5, Reverse = true, Offset = 4)]
        public int ESP_LeaveLength { get; set; }

        #region 简易版将忽略以下属性
        /// <summary>
        /// SOCKET头参数个数(3)
        /// </summary>
        [ObjectTransferOrder(7, Reverse = true, Offset = 4)]
        public short ESP_SocketParamCount { get; set; }

        /// <summary>
        /// 客户端编码格式
        /// </summary>
        [ObjectTransferOrder(9, Reverse = false, Offset = 2)]
        public EaseEncode ESP_EncodeType { get; set; }

        /// <summary>
        /// 本次需返回数据起始位置（首字节从0开始）
        /// </summary>
        [ObjectTransferOrder(11, Reverse = true, Offset = 1)]
        public int ESP_DataIndex { get; set; }

        /// <summary>
        /// 本次需返回数据终止位置(0 – 不断点续传)
        /// </summary>
        [ObjectTransferOrder(13, Reverse = true, Offset = 4)]
        public int ESP_DateEndIndex { get; set; }

        /// <summary>
        /// 应用服务器的地址ID(EASE - 1)
        /// </summary>
        [ObjectTransferOrder(15, Reverse = true, Offset = 4)]
        public short ESP_AppServerID { get; set; }

        /// <summary>
        /// 请求链接	无此参数,则将字符串长度置0	
        /// </summary>
        [ObjectTransferOrder(17, Reverse = false, Offset = 2)]
        public EaseString ESP_Link { get; set; }

        /// <summary>
        /// 应用请求数据长度(通过接入网关转发的数据长度)
        /// </summary>
        [ObjectTransferOrder(19, Reverse = true, Offset = -1)]
        public int ESP_TransferLength { get; set; }
        #endregion

        /// <summary>
        /// 应用请求数据(通过接入网关转发的数据)
        /// </summary>
        [ObjectTransferOrder(21, Reverse = false, Offset = 4)]
        public byte[] ESP_TransferData { get; set; }
        #endregion

        /// <summary>
        /// 获取网络字节序列
        /// </summary>
        /// <returns></returns>
        public override byte[] GetNetworkBytes()
        {
            return GetInstanceNetworkBytes(p =>
            {
                string key = BindBuilder.GetPropertyBindKey(this.GetType(), p);
                return (ESP_SuccessFlag == EaseSuccessFlag.SuccessExpress
                    && BindBuilder.GlobalBindDictinary.ContainsKey(key)
                        && !p.Name.Equals("ESP_TransferData"));
            });
        }

        /// <summary>
        /// 根据网关传递信息获取转发的请求封装[TODO]
        /// </summary>
        /// <returns></returns>
        public RequestBase GetSubRequest()
        {
            System.IO.MemoryStream ms = ESP_TransferData.AsMemoryStream();
            RequestBase subReq = null;
            RequestHeader header = SpecUtil.DataBind<RequestHeader>(ms, 0);
            switch (header.ESP_Method)
            { 
                case RequestType.PageV21 :
                    subReq = new PageV21Request(Context);
                    break;

                case RequestType.Mixed :
                    subReq = new MixedRequest(Context);
                    break;

                case RequestType.Page :
                    subReq = new PageRequest(Context);
                    break;

                case RequestType.Resource :
                    subReq = new ResourceRequest(Context);
                    break;

                case RequestType.Application :
                    subReq = new ApplicationRequest(Context);
                    break;

                case RequestType.UpdateCenter :
                    subReq = new GatewayUpdateRequest(Context);
                    break;

                default :
                    break;
            }
            if (subReq != null)
            {
                subReq.ESP_Header = header;
                SpecUtil.BindFromNetworkStream(subReq, ms, ms.Position, false, 1);
                subReq.ContentRange[0] = 0;
                subReq.ContentRange[1] = ESP_TransferData.LongLength - 1;
            }
            ms.Dispose();
            return subReq;
        }

#if UnitTest
        [Test]
        public void DoTest()
        {
            int i = 0;

        start:

            NetworkSwitchRequest request = new NetworkSwitchRequest();
            request.ESP_SuccessFlag = (i == 0) ? EaseSuccessFlag.SuccessExpress : EaseSuccessFlag.Success;
            request.ESP_CustomeCode = 0;

            request.ESP_TransferData = Encoding.UTF8.GetBytes("123456");

            if (request.ESP_SuccessFlag == EaseSuccessFlag.Success)
            {
                byte[] testLinkBytes = Encoding.UTF8.GetBytes("http://118.123.205.218:888/images/testpic.gif");
                #region 设置完整参数
                request.ESP_SocketParamCount = 3;
                request.ESP_EncodeType = EaseEncode.UTF8;
                request.ESP_DataIndex = 0;
                request.ESP_DateEndIndex = 0;
                request.ESP_AppServerID = 1;
                request.ESP_Link = new EaseString { ESP_Data = testLinkBytes, ESP_Length = (short)testLinkBytes.Length };
                request.ESP_TransferLength = request.ESP_TransferData.Length;
                #endregion
            }

            if (request.ESP_SuccessFlag == EaseSuccessFlag.SuccessExpress)
            {
                request.ESP_LeaveLength = request.ESP_TransferData.Length;
            }
            else
            {
                request.ESP_LeaveLength = 2 + 1 + 4 + 4 + 2 + 4 + (int)request.ESP_Link.GetContentLength(); //后续长度
            }
            //Console.WriteLine("后续长度：{0}", request.ESP_LeaveLength);

            byte[] networkBytes = request.GetNetworkBytes();

            //Console.WriteLine("Total:{0}\r\n{1}", networkBytes.ESP_Length, networkBytes.GetHexViewString());

            NetworkSwitchRequest request2 = new NetworkSwitchRequest();
            MemoryStream ms = new MemoryStream(networkBytes);
            ms.Position = 0;
            request2.BindFromNetworkStream(ms, 0, false);

            //Console.WriteLine("########################################");
            //Console.WriteLine(request2.GetNetworkBytes().GetHexViewString());
            //Console.WriteLine("########################################");

            ms.Dispose();

            Assert.That(SpecUtil.AreEqual(request2.GetNetworkBytes(), networkBytes));

            i++;

            if (i < 2)
            {
                //Console.WriteLine("\r\n下一次测试：\r\n");
                goto start;
            }

        }
#endif

    }
}

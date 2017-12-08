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
    /// EASE网络接入网关响应
    /// </summary>
    [ImplementState(CompleteState.OK, "1.0(v3.1-2.5.2)", Description = "EASE网络接入网关响应", ReleaseDateGTM = "Wed, 30 Dec 2009 18:27:14 GMT")]
#if UnitTest
    [TestFixture]
#endif
    public class NetworkSwitchResponse : ESPDataBase
    {
        /*
            参数名	类型	参数说明
            网络连接成功标志	Int16	1010 - 网络连接成功
            客户端自定义包序号	Int16	回传此参数
            返回包数据长度	Int32	后续字节数
            应用服务器返回数据总长度	Int32	应用服务器返回数据总长度
            本次返回数据起始位置	Int32	本次返回数据起始位置
            本次返回数据终止位置	Int32	本次返回数据起始位置
            应用数据	Bytes	

         */

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkSwitchResponse"/> class.
        /// </summary>
        public NetworkSwitchResponse()
            : base()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkSwitchResponse"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public NetworkSwitchResponse(ESPContext context)
           : base(context)
        { 
        
        }

        #region 属性模型顺序
        /// <summary>
        /// 网络连接成功标志(1010 - 网络连接成功)
        /// </summary>
        [ObjectTransferOrder(0, Reverse = true, Offset = 0)]
        public EaseSuccessFlag ESP_SuccessFlag { get; set; }

        /// <summary>
        /// 客户端自定义包序号（回传此参数）
        /// </summary>
        [ObjectTransferOrder(3, Reverse = true, Offset = 2)]
        public int ESP_CustomCode { get; set; }

        /// <summary>
        /// 返回包数据长度(后续字节数)
        /// </summary>
        [ObjectTransferOrder(5, Reverse = true, Offset = 4)]
        public int ESP_LeaveLength { get; set; }

        #region 简易版将忽略以下属性
        /// <summary>
        /// 应用服务器返回数据总长度
        /// </summary>
        [ObjectTransferOrder(7, Reverse = true, Offset = 4)]
        public int ESP_DataTotalLength { get; set; }

        /// <summary>
        /// 本次需返回数据起始位置（首字节从0开始）
        /// </summary>
        [ObjectTransferOrder(9, Reverse = true, Offset = 4)]
        public int ESP_DataIndex { get; set; }

        /// <summary>
        /// 本次需返回数据终止位置(0 – 不断点续传)
        /// </summary>
        [ObjectTransferOrder(11, Reverse = true, Offset = 4)]
        public int ESP_DateEndIndex { get; set; }
        #endregion

        /// <summary>
        /// 应用数据
        /// </summary>
        [ObjectTransferOrder(13, Reverse = false, Offset = 4)]
        public byte[] ESP_TransferData { get; set; }
        #endregion

        /// <summary>
        /// 获取无效的网关数数据回复
        /// </summary>
        /// <param name="request">网关请求对象</param>
        /// <param name="bizRespBytes">业务返回字节数</param>
        public static NetworkSwitchResponse GetInvalidSwitchResponse(NetworkSwitchRequest request, byte[] bizRespBytes)
        {
            NetworkSwitchResponse resp = new NetworkSwitchResponse();
            resp.ESP_SuccessFlag = EaseSuccessFlag.Error;

            resp.ESP_CustomCode = request.ESP_CustomeCode;
            resp.ESP_LeaveLength = bizRespBytes.Length;

            if (request.ESP_SuccessFlag != EaseSuccessFlag.SuccessExpress)
            {
                resp.ESP_DataTotalLength = bizRespBytes.Length;
                resp.ESP_DataIndex = 0;
                resp.ESP_DateEndIndex = 0;
                resp.ESP_LeaveLength += 4*3;
            }

            resp.ESP_TransferData = bizRespBytes;
            return resp;
        }

        /// <summary>
        /// 填充绑定词典
        /// </summary>
        public override void CustomPropertyBindAction()
        {
            BindBuilder.Instance()
               .Add((NetworkSwitchResponse resp) => resp.ESP_DataTotalLength, //应用服务器返回数据总长度(7)
				(s, l, obj) =>
                {
                    NetworkSwitchResponse cResponse = (NetworkSwitchResponse)obj;
                    if (cResponse.ESP_SuccessFlag == EaseSuccessFlag.Success)
                    {
                        cResponse.ESP_DataTotalLength = s.ReadNetworkStreamAsEntity<int>(4);
                    }
					return new PropertyBindState { PropertyValue = cResponse.ESP_DataTotalLength, StreamBind = true };
                })
                .Add((NetworkSwitchResponse resp) => resp.ESP_DataIndex, //本次需返回数据起始位置（首字节从0开始）(9)
				(s, l, obj) =>
                {
                    NetworkSwitchResponse cResponse = (NetworkSwitchResponse)obj;
                    if (cResponse.ESP_SuccessFlag == EaseSuccessFlag.Success)
                    {
                        cResponse.ESP_DataIndex = s.ReadNetworkStreamAsEntity<int>(4);
                    }
                    return new PropertyBindState { PropertyValue = cResponse.ESP_DataIndex, StreamBind = true };
                })
                .Add((NetworkSwitchResponse resp) => resp.ESP_DateEndIndex, //本次需返回数据终止位置(0 – 不断点续传)(11)
				(s, l, obj) =>
                {
                    NetworkSwitchResponse cResponse = (NetworkSwitchResponse)obj;
                    if (cResponse.ESP_SuccessFlag == EaseSuccessFlag.Success)
                    {
                        cResponse.ESP_DateEndIndex = s.ReadNetworkStreamAsEntity<int>(4);
                    }
                    return new PropertyBindState { PropertyValue = cResponse.ESP_DateEndIndex, StreamBind = true };
                })
                .Add((NetworkSwitchResponse resp) => resp.ESP_TransferData,   /*ESP_TransferData*/
				 (s, l, obj) =>
                 {
                     NetworkSwitchResponse cResponse = (NetworkSwitchResponse)obj;
                     int targetLen = cResponse.ESP_LeaveLength;
                     if (cResponse.ESP_SuccessFlag == EaseSuccessFlag.Success) targetLen = cResponse.ESP_LeaveLength - 4 * 3;
                     //System.Diagnostics.Trace.WriteLine(string.Format("Target:{0}", targetLen), "DEBUG");
                     cResponse.ESP_TransferData = s.ReadNetworkStreamBytes(targetLen, true);
					 return new PropertyBindState { PropertyValue = cResponse.ESP_TransferData, StreamBind = true };
                 })
                .End<NetworkSwitchResponse>();
        }

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
        /// 获取返回包装对象子类(6种Response对象中的一种)
        /// </summary>
        /// <returns></returns>
        public ResponseBase GetSubResponse(RequestType requestType)
        {
            ResponseBase resp = null;
            System.IO.MemoryStream ms = ESP_TransferData.AsMemoryStream();
            switch (requestType)
            {
                case RequestType.PageV21:
                    resp = new PageV21Response(Context);
                    break;
                case RequestType.Mixed:
                    resp = new MixedResponse(Context);
                    break;
                case RequestType.Page:
                    resp = new PageResponse(Context);
                    break;
                case RequestType.Resource:
                    resp = new ResourceResponse(Context);
                    break;
                case RequestType.Application:
                    resp = new ApplicationResponse(Context);
                    break;
                case RequestType.UpdateCenter:
                    resp = new GatewayUpdateResponse(Context);
                    break;
                default:
                    break;
            }

            SpecUtil.BindFromNetworkStream(resp, ms, ms.Position, false, 0);
                resp.ContentRange[0] = 0;
                resp.ContentRange[1] = ESP_TransferData.LongLength - 1;

            
            //System.Diagnostics.Trace.WriteLine(ms.Length);

             ms.Close();
             ms.Dispose();

            return resp;
        }

#if UnitTest
        [Test]
        public void DoTest()
        {
            int i = 0;

        start:

            NetworkSwitchResponse response = new NetworkSwitchResponse();
            response.ESP_SuccessFlag = (i == 0) ? EaseSuccessFlag.SuccessExpress : EaseSuccessFlag.Success;
            response.ESP_CustomCode = 0;

            response.ESP_TransferData = Encoding.UTF8.GetBytes("123456");

            if (response.ESP_SuccessFlag == EaseSuccessFlag.Success)
            {
                #region 设置完整参数
                response.ESP_DataTotalLength = 100;
                response.ESP_DataIndex = 0;
                response.ESP_DateEndIndex = 0;
                #endregion
            }

            if (response.ESP_SuccessFlag == EaseSuccessFlag.SuccessExpress)
            {
                response.ESP_LeaveLength = response.ESP_TransferData.Length;
            }
            else
            {
                response.ESP_LeaveLength = response.ESP_TransferData.Length + 4*3; //后续长度
            }
            //Console.WriteLine("后续长度：{0}", response.ESP_LeaveLength);

            byte[] networkBytes = response.GetNetworkBytes();

            //Console.WriteLine("Total:{0}\r\n{1}", networkBytes.ESP_Length, networkBytes.GetHexViewString());

            NetworkSwitchResponse response2 = new NetworkSwitchResponse();
            MemoryStream ms = new MemoryStream(networkBytes);
            ms.Position = 0;
            response2.BindFromNetworkStream(ms, 0, false);

            //Console.WriteLine("########################################");
            //Console.WriteLine(response2.GetNetworkBytes().GetHexViewString());
            //Console.WriteLine("########################################");

            ms.Dispose();

            Assert.That(SpecUtil.AreEqual(response2.GetNetworkBytes(), networkBytes));

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

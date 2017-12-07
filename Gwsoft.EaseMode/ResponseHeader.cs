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
    /// 业务接入响应头
    /// </summary>
    [ImplementState(CompleteState.OK, "1.0(v3.2-2.6.2)", Description = "EASE网络接入响应头信息", ReleaseDateGTM = "Thu, 31 Dec 2009 15:21:28 GMT")]
#if UnitTest
    [TestFixture]
#endif
    public sealed class ResponseHeader : ESPDataBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseHeader"/> class.
        /// </summary>
        public ResponseHeader()
            : base()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseHeader"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public ResponseHeader(ESPContext context)
            : base(context)
        {
        }

        #region 传输属性
        /// <summary>
        /// 网络连接成功标志(1010 - 网络连接成功)
        /// </summary>
        [ObjectTransferOrder(0, Reverse = true, Offset = 0)]
        public EaseSuccessFlag ESP_SuccessFlag { get; set; }

        /// <summary>
        /// 包后续长度(不包含此参数长度)
        /// </summary>
        [ObjectTransferOrder(1, Reverse = true, Offset = 2)]
        public int ESP_LeaveLength { get; set; }

        /// <summary>
        /// 软件ID
        /// </summary>
        [ObjectTransferOrder(2, Reverse = true, Offset = 4)]
        public int ESP_SoftwareID { get; set; }

        /// <summary>
        /// 会话标识（不大于50）
        /// </summary>
        [ObjectTransferOrder(3, Reverse = false, Offset = 4)]
        public EaseString ESP_SessionID { get; set; }


        /// <summary>
        /// 请求功能号0－页面请求,兼容EASE 2.1
        /// <para> 1－页面及资源请求  2－页面请求 </para>
        /// <para> 3－资源请求  4－应用请求</para>
        /// </summary>
        [ObjectTransferOrder(4, Reverse = false, Offset = -1)]
        public RequestType ESP_Method { get; set; }
        #endregion

#if UnitTest
        private static ResponseHeader _responseHeader4Test = null;

        public static ResponseHeader ResponseHeader4Test
        {
            get {
                if (_responseHeader4Test == null)
                {
                    ResponseHeader response = new ResponseHeader();
                    response.ESP_SuccessFlag = EaseSuccessFlag.Success;
                    response.ESP_SoftwareID = 10000;
                    byte[] cookieBytes = Encoding.UTF8.GetBytes("abcdef");
                    response.ESP_SessionID = new EaseString { ESP_Data = cookieBytes, ESP_Length = (short)cookieBytes.Length };
                    response.ESP_Method = RequestType.UpdateCenter;

                    response.ESP_LeaveLength = 4 + (int)response.ESP_SessionID.GetContentLength()
                        + 1;

                    //Console.WriteLine("ESP_LeaveLength : {0}:{1}", response.ESP_LeaveLength, response.ESP_LeaveLength.ToString("X2"));

                    _responseHeader4Test = response;
                }
                return _responseHeader4Test;
            }
        }

        [Test]
        public void DoTest()
        {
            ResponseHeader response = ResponseHeader4Test;

            byte[] networkBytes = response.GetNetworkBytes();

            //Console.WriteLine("Total:{0}\r\n{1}", networkBytes.ESP_Length, networkBytes.GetHexViewString());

            ResponseHeader response2 = new ResponseHeader();
            System.IO.MemoryStream ms = new System.IO.MemoryStream(networkBytes);
            ms.Position = 0;
            response2.BindFromNetworkStream(ms, 0, false);

            //Console.WriteLine("########################################");
            //Console.WriteLine(response2.GetNetworkBytes().GetHexViewString());
            //Console.WriteLine("########################################");

            ms.Dispose();

            Assert.That(SpecUtil.AreEqual(response2.GetNetworkBytes(), networkBytes));
        }
#endif

    }
}

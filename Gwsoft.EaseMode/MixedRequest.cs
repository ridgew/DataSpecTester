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
    /// 页面文档及资源请求
    /// </summary>
    [ImplementState(CompleteState.OK, "1.0(v3.1-2.6.4.1)", Description = "页面文档及资源请求", ReleaseDateGTM = "Mon, 04 Jan 2010 16:57:58 GMT")]
    public class MixedRequest : RequestBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MixedRequest"/> class.
        /// </summary>
        public MixedRequest()
            : base()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MixedRequest"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public MixedRequest(ESPContext context)
            : base(context)
        { }

        #region 属性模型顺序
        /// <summary>
        /// 页面链接
        /// </summary>
        /// 页面链接
        /// </summary>
        [ObjectTransferOrder(3, Reverse = false, Offset = -1)]
        public EaseString ESP_Link { get; set; }
        #endregion
    }


    /// <summary>
    /// 页面文档及资源响应
    /// </summary>
    [ImplementState(CompleteState.OK, "1.0(v3.1-2.6.4.2)", Description = "页面文档及资源响应", ReleaseDateGTM = "Mon, 04 Jan 2010 18:02:17 GMT")]
#if UnitTest
    [TestFixture]
#endif
    public class MixedResponse : ResponseBase
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="MixedResponse"/> class.
        /// </summary>
        public MixedResponse()
            : base()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MixedResponse"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public MixedResponse(ESPContext context)
            : base(context)
        { }

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

        /// <summary>
        /// 资源文件数
        /// </summary>
        [ObjectTransferOrder(22, Reverse = true, Offset = -1)]
        public short ESP_PageResCount { get; set; }

        /// <summary>
        /// 资源文件内容
        /// </summary>
        [ObjectTransferOrder(23, Reverse = false, Offset = 2)]
        public EaseResource[] ESP_Resources { get; set; }
        #endregion

        ///// <summary>
        ///// 自定义属性绑定词典
        ///// </summary>
        //public override void CustomPropertyBindAction()
        //{
        //    BindBuilder.Instance()
        //        .Add((MixedResponse resp) => resp.ESP_Docs, //文档对象绑定
        //         (s, obj) =>
        //         {
        //             MixedResponse cResp = (MixedResponse)obj;
        //             cResp.ESP_Docs = s.GetCurrentContainerEntities<EaseDocument, MixedResponse>(cResp, r => (int)r.ESP_PageDocCount);
        //             return cResp.ESP_Docs;
        //         })
        //         .Add((MixedResponse resp) => resp.ESP_Resources, //资源对象绑定
        //         (s, obj) =>
        //         {
        //             MixedResponse cResp = (MixedResponse)obj;
        //             cResp.ESP_Resources = s.GetCurrentContainerEntities<EaseResource, MixedResponse>(cResp, r => (int)r.ESP_PageResCount);
        //             return cResp.ESP_Resources;
        //         })
        //         .End<MixedResponse>();
        //}


#if UnitTest


        [Test]
        public void DoTest()
        {
            //ESP_Type lastObjDateType = typeof(byte[]);
            //byte[] lastDat = new byte[] { 0x00, 0x01 };
            //long lastOffset = Convert.ToInt64(lastObjDateType.GetProperty("ESP_Length").GetValue(lastDat, null));
            //Console.WriteLine(lastOffset);


            MixedResponse instance = new MixedResponse();
            instance.ESP_Header = ResponseHeader.ResponseHeader4Test;
            instance.ESP_Code = StatusCode.Success;
            byte[] msgBytes = EaseString.DefaultEncoding.GetBytes("OK");
            instance.ESP_Message = new EaseString { ESP_Data = msgBytes, ESP_Length = (short)msgBytes.Length };
            
            instance.ESP_Method = CommandType.None;

            byte[] cmdBytes = EaseString.DefaultEncoding.GetBytes("GET / HTTP/1.1");
            instance.ESP_Command = new EaseString { ESP_Data = cmdBytes, ESP_Length = (short)cmdBytes.Length };

            instance.ESP_PageDocCount = 2;
            instance.ESP_Docs = new EaseDocument[] { EaseDocument.EaseDocument4Test, EaseDocument.EaseDocument4Test };

            instance.ESP_PageResCount = 2;
            instance.ESP_Resources = new EaseResource[] { EaseResource.EaseResource4Test, EaseResource.EaseResource4Test };


            byte[] testBytes = instance.GetNetworkBytes();
            //Console.WriteLine("Total:{0}\r\n{1}", testBytes.ESP_Length, testBytes.GetHexViewString());

            MixedResponse instanceCmp = new MixedResponse();
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

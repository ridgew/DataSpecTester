using System;
using System.Collections.Generic;
using System.Text;
using Gwsoft.DataSpec;
using Gwsoft.Configuration;

#if UnitTest
using NUnit.Framework;
using System.IO;
#endif
namespace Gwsoft.EaseMode
{
    /// <summary>
    /// Ease文档
    /// </summary>
#if UnitTest
    [TestFixture]
#endif
    [ImplementState(CompleteState.OK, "1.0(v3.2-2.3.3)", Description = "资源文档装", ReleaseDateGTM = "Wed, 30 Dec 2009 10:08:12 GMT")]
    public class EaseDocument : ESPDataBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EaseDocument"/> class.
        /// </summary>
        public EaseDocument()
            : base()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="EaseDocument"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public EaseDocument(ESPContext context)
            : base(context)
        { }

        #region 传输属性
        /// <summary>
        /// 页面类型：0: 下载页面 1: 显示页面 2：调用本地页面
        /// </summary>
        [ObjectTransferOrder(0, Reverse = false, Offset = 0)]
        public DocumentType ESP_Type { get; set; }

        /// <summary>
        /// 页面名称
        /// </summary>
        [ObjectTransferOrder(1, Reverse = false, Offset = 1)]
        public EaseString ESP_Name { get; set; }

        /// <summary>
        /// 页面地址
        /// </summary>
        [ObjectTransferOrder(2, Reverse = false, Offset = -1)]
        public EaseString ESP_URL { get; set; }

        /// <summary>
        /// 版本号
        /// </summary>
        [ObjectTransferOrder(3, Reverse = true, Offset = -1)]
        public Int32 ESP_Version { get; set; }

        /// <summary>
        /// 文档内容
        /// </summary>
        [ObjectTransferOrder(4, Reverse = false, Offset = 4)]
        public EaseString ESP_Content { get; set; }
        #endregion

#if UnitTest
        private static EaseDocument _EaseDocument4Test = null;

        public static EaseDocument EaseDocument4Test
        {
            get
            {
                if (_EaseDocument4Test == null)
                {
                    byte[] nameBytes = Encoding.UTF8.GetBytes("测试游戏网页文档");
                    byte[] urlBytes = Encoding.UTF8.GetBytes("http://118.123.205.218:888/mgame/testgame.aspx?机型=中文");
                    byte[] contentBytes = Encoding.UTF8.GetBytes("<html>hello world!</html>");

                    EaseDocument doc = new EaseDocument
                    {
                        ESP_Type = DocumentType.Download,
                        ESP_Name = new EaseString { ESP_Data = nameBytes, ESP_Length = (short)nameBytes.Length },
                        ESP_URL = new EaseString { ESP_Data = urlBytes, ESP_Length = (short)urlBytes.Length },
                        ESP_Version = 0,
                        ESP_Content = new EaseString { ESP_Data = contentBytes, ESP_Length = (short)contentBytes.Length }
                    };

                    _EaseDocument4Test = doc;
                }
                return _EaseDocument4Test;
            }
        }

        [Test]
        public void DoTest()
        {
            EaseDocument doc = EaseDocument4Test;
            byte[] networkBytes = doc.GetNetworkBytes();

            EaseDocument doc2 = new EaseDocument();
            MemoryStream ms = new MemoryStream(networkBytes);
            ms.Position = 0;

            doc2.BindFromNetworkStream(ms, 0, false);

            Assert.That(doc2.ESP_URL.Equals(doc.ESP_URL));
            Assert.That(doc2.ESP_URL.GetRawString() == "http://118.123.205.218:888/mgame/testgame.aspx?机型=中文");

            Assert.That(doc.ESP_Type == doc2.ESP_Type
                && doc.ESP_Name.Equals(doc2.ESP_Name)
                && doc.ESP_Version == doc2.ESP_Version
                && doc2.ESP_Version == 0
                );

            Assert.That(doc.ESP_Content.Equals(doc2.ESP_Content), "文档内容不一致！");


        }
#endif

    }

    /// <summary>
    /// 文档类型(byte)
    /// </summary>
    public enum DocumentType : byte
    {
        /// <summary>
        /// 下载页面
        /// </summary>
        Download = 0,
        /// <summary>
        /// 显示页面
        /// </summary>
        Render = 1,
        /// <summary>
        /// 本地调用页面
        /// </summary>
        Local = 2
    }
}

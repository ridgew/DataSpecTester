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
    /// Ease资源文件
    /// </summary>
#if UnitTest
    [TestFixture]
#endif
    [ImplementState(CompleteState.OK, "1.0(v3.2-2.3.4)", Description = "资源文件封装", ReleaseDateGTM = "Wed, 30 Dec 2009 10:08:12 GMT")]
    public class EaseResource : ESPDataBase
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="EaseResource"/> class.
        /// </summary>
        public EaseResource()
            : base()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EaseString"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public EaseResource(ESPContext context)
            : base(context)
        {

        }


        #region 传输属性
        /// <summary>
        /// 资源名称
        /// </summary>
        [ObjectTransferOrder(0, Reverse = false, Offset = 0)]
        public EaseString ESP_Name { get; set; }

        /// <summary>
        /// 资源地址 http://....  file:///....
        /// </summary>
        [ObjectTransferOrder(1, Reverse = false, Offset = -1)]
        public EaseString ESP_URL { get; set; }

        /// <summary>
        /// 资源类型
        /// </summary>
        [ObjectTransferOrder(2, Reverse = true, Offset = -1)]
        public ResourceCatelog ESP_Catelog { get; set; }

        /// <summary>
        /// 资源文档长度
        /// </summary>
        [ObjectTransferOrder(3, Reverse = true, Offset = 2)]
        public Int32 ESP_Length { get; set; }

        /// <summary>
        /// 资源文件数据
        /// </summary>
        [ObjectTransferOrder(4, Reverse = false, Offset = 4)]
        public byte[] ESP_Data { get; set; }
        #endregion

        ///// <summary>
        ///// 自定义属性绑定词典
        ///// </summary>
        //public override void CustomPropertyBindAction()
        //{

        //    BindBuilder.Instance()
        //        .Add((EaseResource res) => res.ESP_Data,
        //        (s, obj) =>
        //        {
        //            EaseResource target = (EaseResource)obj;
        //            target.ESP_Data = s.ReadNetworkStreamBytes(target.ESP_Length);
        //            return target.ESP_Data;
        //        })
        //        .End<EaseResource>();
        //}

#if UnitTest
        private static EaseResource _easeResource4Test = null;

        public static EaseResource EaseResource4Test
        {
            get {
                if (_easeResource4Test == null)
                {
                    byte[] dat = Encoding.UTF8.GetBytes("游戏图片测试");
                    EaseString strName = new EaseString { ESP_Length = (short)dat.Length, ESP_Data = dat };

                    byte[] urlDat = EaseString.DefaultEncoding.GetBytes("http://118.123.205.218:888/images/testpic.gif");
                    EaseString strURL = new EaseString { ESP_Length = (short)urlDat.Length, ESP_Data = urlDat };

                    //Console.WriteLine("{0}={1}", str.ESP_Length, dat.ESP_Length);
                    EaseResource res = new EaseResource();
                    res.ESP_Name = strName;
                    res.ESP_URL = strURL;

                    res.ESP_Catelog = ResourceCatelog.Picture;
                    res.ESP_Data = Encoding.ASCII.GetBytes("gif89a");
                    res.ESP_Length = res.ESP_Data.Length;

                    _easeResource4Test = res;
                }
                return _easeResource4Test;
            }
        }

        [Test]
        public void DoTest()
        {

            EaseResource res = EaseResource4Test;
            byte[] networkBytes = res.GetNetworkBytes();


            //Console.WriteLine("Len: {1}, Network Bytes: {0}", BitConverter.ToString(networkBytes), networkBytes.ESP_Length);

            EaseResource str2 = new EaseResource();
            MemoryStream ms = new MemoryStream(networkBytes);
            ms.Position = 0;

            //Console.WriteLine("网络字节流，长度:{0}\r\n{1}", networkBytes.ESP_Length, networkBytes.GetHexViewString());

            if (!HasImplementDataBind)
            {
                str2.BindFromNetworkStream(ms, 0, false);
            }
            else
            {
                str2.BindMappingWithStream(ms);
            }

            Assert.That(res.ESP_Name.Equals(str2.ESP_Name));
            Assert.That(res.ESP_URL.Equals(str2.ESP_URL));
            Assert.That(res.ESP_Length == str2.ESP_Length);
            Assert.That(res.ESP_Catelog == str2.ESP_Catelog);

            //Console.WriteLine(str2.ESP_Data.GetHexViewString());

            Assert.That(SpecUtil.AreEqual(res.ESP_Data, str2.ESP_Data));
        }
#endif

    }

    /// <summary>
    /// 资源类型
    /// </summary>
    public enum ResourceCatelog : short
    {
        /// <summary>
        /// 未知资源类型
        /// </summary>
        UnKnown = 0,

        /// <summary>
        /// 图片
        /// </summary>
        Picture = 1,
        /// <summary>
        /// 铃声
        /// </summary>
        Ring = 2
    }
}

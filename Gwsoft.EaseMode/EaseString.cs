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
    /// Ease字符串(不定长）
    /// </summary>
#if UnitTest
    [TestFixture]
#endif
    [ImplementState(CompleteState.OK, "1.0(v3.2-2.3.1)", Description = "字符串封装", ReleaseDateGTM = "Wed, 30 Dec 2009 10:08:12 GMT")]
    public class EaseString : ESPDataBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EaseString"/> class.
        /// </summary>
        public EaseString()
            : base()
        {
            internalImplementDataBind = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EaseString"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public EaseString(ESPContext context)
            : base(context)
        {
            internalImplementDataBind = true;
        }

        #region 传输属性
        /// <summary>
        /// 字符串为空时，此处为0
        /// </summary>
        [ObjectTransferOrder(0, Reverse = true, Offset = 0)]
        public Int16 ESP_Length { get; set; }

        /// <summary>
        /// 字符串为空时，此处不存在
        /// </summary>
        [ObjectTransferOrder(1, Reverse = false, Offset = 2)]
        public byte[] ESP_Data { get; set; }

        #endregion
        /// <summary>
        /// 字符串封装的默认编码
        /// </summary>
        public static Encoding DefaultEncoding
        {
            get { return Encoding.UTF8; }
        }

        /// <summary>
        /// 没有内容的字符串
        /// </summary>
        public static EaseString Empty = new EaseString { ESP_Data = new byte[0], ESP_Length = 0 };

        /// <summary>
        /// 获取原生字符串的封装格式
        /// </summary>
        public static EaseString Get(string rawString)
        {
            byte[] binDat = DefaultEncoding.GetBytes(rawString ?? string.Empty).TrimStart(SpecUtil.UTF8_BOM_BYTES);
            return new EaseString { ESP_Data = binDat, ESP_Length = (short)binDat.Length };
        }

        /// <summary>
        /// 获取POCO字符串内容
        /// </summary>
        /// <returns></returns>
        public string GetRawString()
        {
            string strResult = null;
            if (ESP_Data != null && ESP_Data.Length > 0)
            {
                strResult = DefaultEncoding.GetString(ESP_Data);
            }
            return strResult;
        }

        /// <summary>
        /// 获取字节序列总长度
        /// </summary>
        /// <returns></returns>
        public override long GetContentLength()
        {
            return 2L + Convert.ToInt64(ESP_Length);
        }

        /// <summary>
        /// 绑定内置数据
        /// </summary>
        /// <param name="mapStm"></param>
        public override void BindMappingWithStream(System.IO.Stream mapStm)
        {
            JustMapped = false;

            SpecUtil.TryDoStreamAction(mapStm, s =>
            {
                ContentRange[0] = s.Position;
            });

            ESP_Length = mapStm.ReadNetworkStreamAsEntity<short>(2);
            ESP_Data = mapStm.ReadNetworkStreamBytes(ESP_Length);

            SpecUtil.TryDoStreamAction(mapStm, s =>
            {
                ContentRange[1] = s.Position;
            });

            if (ContentRange[1] > ContentRange[0])
                ContentRange[1] = ContentRange[1] - 1;

            //Console.WriteLine("读取字符串长度为:{0}, 绑定内容位置索引:{1}:{3}-{2}:{4}", ESP_Length,
            //    ContentRange[0].ToString("X2").PadLeft(8, '0'),
            //    ContentRange[1].ToString("X2").PadLeft(8, '0'),
            //    ContentRange[0],
            //    ContentRange[1]);

            //修正长度逻辑
            if (ContentRange[0] > -1 && (ContentRange[1] - ContentRange[0] + 1 < (long)ESP_Length))
            {
                ESP_Length = (short)(ContentRange[1] - ContentRange[0] + 1);
                SpecUtil.TryDoStreamAction(mapStm, s =>
                {
                    mapStm.Position = ContentRange[0];
                });
                ESP_Data = mapStm.ReadNetworkStreamBytes(ESP_Length);
                //Console.WriteLine("内部修正长度为:{0}", ESP_Length);
            }

        }

        /// <summary>
        /// 获取网络字节序列
        /// </summary>
        /// <returns></returns>
        public override byte[] GetNetworkBytes()
        {
            byte[] retBytes = new byte[0];

            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            byte[] lenBytes = SpecUtil.ReverseBytes(BitConverter.GetBytes(ESP_Length));
            ms.Write(lenBytes, 0, lenBytes.Length);
            if (ESP_Length > 0)
            {
                ms.Write(ESP_Data, 0, ESP_Data.Length);
            }
            retBytes = ms.ToArray();
            ms.Dispose();

            return retBytes;
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        /// The <paramref name="obj"/> parameter is null.
        /// </exception>
        public override bool Equals(object obj)
        {
            if (obj != null && obj.GetType() == this.GetType())
            {
                EaseString target = (EaseString)obj;
                return (target.ESP_Length == this.ESP_Length &&
                    SpecUtil.AreEqual(target.ESP_Data, this.ESP_Data));
            }
            return base.Equals(obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// 返回表示当前 <see cref="T:System.Object"/> 的 <see cref="T:System.String"/>。
        /// </summary>
        /// <returns>
        /// 	<see cref="T:System.String"/>，表示当前的 <see cref="T:System.Object"/>。
        /// </returns>
        public override string ToString()
        {
            return string.Format("[Length:{0},Value:{1}]", ESP_Length, GetRawString());
        }

#if UnitTest
        [Test]
        public void DoTest()
        {
            byte[] dat = Encoding.UTF8.GetBytes("测试");
            EaseString str = new EaseString { ESP_Length = (short)dat.Length, ESP_Data = dat };

            //Console.WriteLine("{0}={1}", str.ESP_Length, dat.ESP_Length);

            byte[] networkBytes = str.GetNetworkBytes();
            //Console.WriteLine("Len: {1}, Network Bytes: {0}", BitConverter.ToString(networkBytes), networkBytes.ESP_Length);
            //Console.WriteLine("Total:{0}\r\n{1}", networkBytes.ESP_Length, networkBytes.GetHexViewString());

            EaseString str2 = new EaseString();
            MemoryStream ms = new MemoryStream(networkBytes);
            ms.Position = 0;

            if (!HasImplementDataBind)
            {
                str2.BindFromNetworkStream(ms, 0, false);
            }
            else
            {
                str2.BindMappingWithStream(ms);
            }

            byte[] bytes2cmp = str2.GetNetworkBytes();
            //Console.WriteLine("Cmp Total:{0}\r\n{1}", bytes2cmp.ESP_Length, bytes2cmp.GetHexViewString());

            Assert.That(str.ESP_Length == str2.ESP_Length);
            Assert.That(SpecUtil.AreEqual(str.ESP_Data, str2.ESP_Data));
            Assert.That(str.Equals(str2));
        }
#endif
    }
}

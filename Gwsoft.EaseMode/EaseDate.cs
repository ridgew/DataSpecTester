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
    /// Ease日期
    /// </summary>
#if UnitTest
    [TestFixture]
#endif
    [ImplementState(CompleteState.OK, "1.0(v3.2-2.3.2)", Description = "日期封装", ReleaseDateGTM = "Wed, 30 Dec 2009 10:08:12 GMT")]
    public class EaseDate : ESPDataBase
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="EaseDate"/> class.
        /// </summary>
        public EaseDate()
            : base()
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="EaseDate"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public EaseDate(ESPContext context)
            : base(context)
        {
        }

        #region 传输属性
        /// <summary>
        /// 年
        /// </summary>
        [ObjectTransferOrder(0, Reverse = true, Offset = 0)]
        public Int16 ESP_Year { get; set; }

        /// <summary>
        /// 月
        /// </summary>
        [ObjectTransferOrder(1, Reverse = false, Offset = 2)]
        public byte ESP_Month { get; set; }

        /// <summary>
        /// 日
        /// </summary>
        [ObjectTransferOrder(2, Reverse = false, Offset = 1)]
        public byte ESP_Day { get; set; }

        /// <summary>
        /// 时
        /// </summary>
        [ObjectTransferOrder(3, Reverse = false, Offset = 1)]
        public byte ESP_Hour { get; set; }

        /// <summary>
        /// 分
        /// </summary>
        [ObjectTransferOrder(4, Reverse = false, Offset = 1)]
        public byte ESP_Minute { get; set; }
        #endregion

        /// <summary>
        /// 用作特定类型的哈希函数。
        /// </summary>
        /// <returns>当前 <see cref="T:System.Object"/> 的哈希代码。</returns>
        public override int GetHashCode()
        {
            return ESP_Year.GetHashCode() | ESP_Month.GetHashCode() | ESP_Day.GetHashCode() | ESP_Hour.GetHashCode() | ESP_Minute.GetHashCode();
        }

        /// <summary>
        /// 确定指定的 <see cref="T:System.Object"/> 是否等于当前的 <see cref="T:System.Object"/>。
        /// </summary>
        /// <param name="obj">与当前的 <see cref="T:System.Object"/> 进行比较的 <see cref="T:System.Object"/>。</param>
        /// <returns>
        /// 如果指定的 <see cref="T:System.Object"/> 等于当前的 <see cref="T:System.Object"/>，则为 true；否则为 false。
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        /// 	<paramref name="obj"/> 参数为 null。
        /// </exception>
        public override bool Equals(object obj)
        {
            if (obj != null && obj.GetType() == this.GetType())
            {
                EaseDate target = (EaseDate)obj;

                return (target.ESP_Year == this.ESP_Year 
                    && target.ESP_Month == this.ESP_Month
                    && target.ESP_Day == this.ESP_Day
                    && target.ESP_Hour == this.ESP_Hour
                    && target.ESP_Minute == this.ESP_Minute);
            }
            return base.Equals(obj);
        }

#if UnitTest
        [Test]
        public void DoTest()
        {
            EaseDate str = new EaseDate { ESP_Year = 2009, ESP_Month = 12, ESP_Day = 30, ESP_Hour = 9, ESP_Minute = 58 };

            byte[] networkBytes = str.GetNetworkBytes();
            //Console.WriteLine("Len: {1}, Network Bytes: {0}", BitConverter.ToString(networkBytes), networkBytes.ESP_Length);

            EaseDate str2 = new EaseDate();
            MemoryStream ms = new MemoryStream(networkBytes);
            ms.Position = 0;

           str2.BindFromNetworkStream(ms, 0, false);

           Assert.That(str.Equals(str2));
           Assert.That(str.GetHashCode() == str2.GetHashCode());

           //Console.WriteLine("Date:{0}", str.GetHashCode());
        }
#endif

    }
}

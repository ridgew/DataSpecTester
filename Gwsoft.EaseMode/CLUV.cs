using System;
using System.IO;
using System.Text;
using EaseServer.Configuration;

namespace EaseServer.ESP
{
    /// <summary>
    /// 不定长度Unicode字符串:{Unicode字符串长度;数据内容(不包含结束符);} 2 + length
    /// </summary>
    public class CLUV : ESPDataBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CLUV"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public CLUV(ESPContext context)
            : base(context)
        { }

        #region 传输属性
        /// <summary>
        /// 字节流长度
        /// </summary>
        [ObjectTransferOrder(0, Reverse = true, Offset = 0)]
        [CLSCompliant(false)]
        public ushort ByteLength
        {
            get;
            set;
        }

        [CLSCompliant(false)]
        protected byte[] _sBytes = new byte[0];

        /// <summary>
        /// 字符字节序列流
        /// </summary>
        [ObjectTransferOrder(1, Reverse = false, Offset = 2)]
        public byte[] StringBytes
        {
            get { return _sBytes; }
            set { _sBytes = value; }
        }
        #endregion

        /// <summary>
        /// 获取不定长字符串(Unicode编码)的实际内容
        /// </summary>
        public virtual string GetContent()
        {
            if (_sBytes.Length > 0)
            {
                return Encoding.Unicode.GetString(_sBytes);
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 获取字节序列总长度
        /// </summary>
        /// <returns></returns>
        public override long GetRangeLength()
        {
            return 2L + Convert.ToInt64(ByteLength);
        }

        /// <summary>
        /// Binds the index of the stream with begin.
        /// </summary>
        /// <param name="readableStm">The readable STM.</param>
        /// <param name="posBeginAt">The pos begin at.</param>
        /// <param name="isJustMapingBind">if set to <c>true</c> [is just maping bind].</param>
        public override void BindStreamWithBeginIndex(Stream readableStm, long posBeginAt, bool isJustMapingBind)
        {
            byte[] buffer = new byte[2];
            readableStm.Read(buffer, 0, 2);

            object dat = Global.GetDataFromReverseBytes(buffer, true, (rdr) => rdr.ReadUInt16());
            ByteLength = Convert.ToUInt16(dat);

            MappingRange[0] = posBeginAt;
            MappingRange[1] = posBeginAt + GetRangeLength() - 1;

            if (isJustMapingBind)
            {
                readableStm.Position = MappingRange[1] + 1;
            }
            else
            {
                StringBytes = new byte[ByteLength];
                readableStm.Read(StringBytes, 0, Convert.ToInt32(ByteLength));
            }
        }

        /// <summary>
        /// 绑定内置数据
        /// </summary>
        /// <param name="mapStm"></param>
        public override void BindMappingWithStream(Stream mapStm)
        {
            JustMapped = false;

            SetCurrentPosition(mapStm);

            byte[] buffer = new byte[2];
            mapStm.Position = MappingRange[0];
            mapStm.Read(buffer, 0, 2);

            object dat = Global.GetDataFromReverseBytes(buffer, true, (rdr) => rdr.ReadUInt16());
            ByteLength = Convert.ToUInt16(dat);

            StringBytes = new byte[ByteLength];
            mapStm.Read(StringBytes, 0, Convert.ToInt32(ByteLength));

            RestorePosition(mapStm);
        }

        /// <summary>
        /// 返回表示当前 <see cref="T:System.Object"/> 的 <see cref="T:System.String"/>。
        /// </summary>
        /// <returns>
        /// 	<see cref="T:System.String"/>，表示当前的 <see cref="T:System.Object"/>。
        /// </returns>
        public override string ToString()
        {
            return GetContent();
        }

    }

}

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using EaseServer.Configuration;

namespace EaseServer.ESP
{
    /// <summary>
    /// 二进制数据流:{数据长度;数据内容;} 4 + length
    /// </summary>
    public class BLV : ESPDataBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BLV"/> class.
        /// </summary>
        public BLV(ESPContext context)
            : base(context)
        {
            this._transBytes = typeof(uint).GetNetworkBytes(0);
        }

        #region 传输属性
        /// <summary>
        /// 获取或设置二进制字节流的总长度
        /// </summary>
        [ObjectTransferOrder(0, Reverse = true, Offset = 0)]
        [CLSCompliant(false)]
        public uint Length
        {
            get;
            set;
        }

        /// <summary>
        /// 二进制数据内容
        /// </summary>
        [ObjectTransferOrder(1, Reverse = false, Offset = 4)]
        public byte[] DataContent
        {
            get;
            set;
        }
        #endregion

        /// <summary>
        /// 获取字节序列总长度
        /// </summary>
        /// <returns></returns>
        public override long GetRangeLength()
        {
            return 4L + Convert.ToInt64(Length);
        }

        /// <summary>
        /// 从制定索引处绑定数据
        /// </summary>
        /// <param name="readableStm">可读的字节流</param>
        /// <param name="posBeginAt">开始位置</param>
        /// <param name="isJustMapingBind">是否只绑定数据索引映射信息</param>
        public override void BindStreamWithBeginIndex(Stream readableStm, long posBeginAt, bool isJustMapingBind)
        {
            byte[] buffer = new byte[4];
            readableStm.Read(buffer, 0, 4);

            object dat = Global.GetDataFromReverseBytes(buffer, true, (rdr) => rdr.ReadUInt32());
            Length = Convert.ToUInt32(dat);

            MappingRange[0] = posBeginAt;
            MappingRange[1] = posBeginAt + GetRangeLength() - 1;

            if (isJustMapingBind)
            {
                readableStm.Position = MappingRange[1] + 1;
            }
            else
            {
                DataContent = new byte[Length];
                readableStm.Read(DataContent, 0, Convert.ToInt32(Length));
            }
        }

        /// <summary>
        /// Binds the mapping with stream.
        /// </summary>
        /// <param name="mapStm">The map STM.</param>
        public override void BindMappingWithStream(Stream mapStm)
        {
            JustMapped = false;
            SetCurrentPosition(mapStm);

            byte[] buffer = new byte[4];
            mapStm.Position = MappingRange[0];
            mapStm.Read(buffer, 0, 4);

            object dat = Global.GetDataFromReverseBytes(buffer, true, (rdr) => rdr.ReadUInt32());
            Length = Convert.ToUInt32(dat);

            DataContent = new byte[Length];
            mapStm.Read(DataContent, 0, Convert.ToInt32(Length));

            RestorePosition(mapStm);
        }

    }

}

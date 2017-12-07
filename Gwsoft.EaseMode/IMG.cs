using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using EaseServer.Configuration;

namespace EaseServer.ESP
{
    /// <summary>
    /// 图片数据流:{1png|2jpg|3bmp|4gif|5ico图片二进制数据内容} 
    /// 当type=0时 data为空
    /// (1 + data)
    /// </summary>
    public class IMG : ESPDataBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IMG"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public IMG(ESPContext context)
            : base(context)
        { }

        #region 传输属性
        /// <summary>
        /// 图片类型
        /// </summary>
        [ObjectTransferOrder(0, Reverse = false, Offset = 0)]
        public ImageType Type
        {
            get;
            set;
        }

        /// <summary>
        /// 二进制数据流内容
        /// </summary>
        [ObjectTransferOrder(1, Reverse = false, Offset = 1)]
        public BLV BinaryDat
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
            return 1L + (BinaryDat != null ? BinaryDat.GetRangeLength() : 0L);
        }

        /// <summary>
        /// 从制定索引处绑定数据
        /// </summary>
        /// <param name="readableStm">可读的字节流</param>
        /// <param name="posBeginAt">开始位置</param>
        /// <param name="isJustMapingBind">是否只绑定数据索引映射信息</param>
        public override void BindStreamWithBeginIndex(Stream readableStm, long posBeginAt, bool isJustMapingBind)
        {
            MappingRange[0] = posBeginAt;
            JustMapped = isJustMapingBind;

            byte tByte = (byte)readableStm.ReadByte();
            Type = (ImageType)tByte;
            if (Type == ImageType.NULL)
            {
                BinaryDat = null;
            }
            else
            {
                BLV bData = new BLV(Context);
                bData.BindStreamWithBeginIndex(readableStm, readableStm.Position, isJustMapingBind);
                BinaryDat = bData;
            }
            MappingRange[1] = readableStm.Position - 1;
        }

        /// <summary>
        /// Binds the mapping with stream.
        /// </summary>
        /// <param name="mapStm">The map STM.</param>
        public override void BindMappingWithStream(Stream mapStm)
        {
            JustMapped = false;
            SetCurrentPosition(mapStm);

            mapStm.Position = MappingRange[0];
            byte tByte = (byte)mapStm.ReadByte();
            Type = (ImageType)tByte;
            if (Type == ImageType.NULL)
            {
                BinaryDat = null;
            }
            else
            {
                BLV bData = new BLV(Context);
                bData.BindMappingWithStream(mapStm);
                BinaryDat = bData;
            }
            RestorePosition(mapStm);
        }


    }
}

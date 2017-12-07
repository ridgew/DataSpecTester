using System;
using System.Collections.Generic;
using System.Text;
using EaseServer.Configuration;

namespace EaseServer.ESP
{
    /// <summary>
    /// 音乐结构
    /// </summary>
    public class MUSIC : ESPDataBase
    {
        public MUSIC(ESPContext context)
            : base(context)
        { 
        
        }

        /// <summary>
        /// 音乐ID
        /// </summary>
        [CLSCompliant(false)]
        [ObjectTransferOrder(0, Reverse = true, Offset = 0)]
        public UInt32 MusiceId { get; set; }

        /// <summary>
        /// 音乐名称
        /// </summary>
        [ObjectTransferOrder(1, Reverse = false, Offset = -1)]
        public CLUV Name { get; set; }

        /// <summary>
        /// 歌手名称
        /// </summary>
        [ObjectTransferOrder(2, Reverse = false, Offset = -1)]
        public CLUV Singer { get; set; }

        /// <summary>
        /// 专辑名
        /// </summary>
        [ObjectTransferOrder(3, Reverse = false, Offset = -1)]
        public CLUV SpecialName { get; set; }

        /// <summary>
        /// 音乐时间长度
        /// </summary>
        [ObjectTransferOrder(4, Reverse = false, Offset = -1)]
        public CLUV Time { get; set; }

    }
}

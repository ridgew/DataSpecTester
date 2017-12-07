using System;
using System.Collections.Generic;
using System.Text;
using EaseServer.Configuration;

namespace EaseServer.ESP
{
    /// <summary>
    /// 歌曲结构
    /// </summary>
    public class SONG : ESPDataBase
    {
        public SONG(ESPContext context)
            : base(context)
        { 
        
        }

        /// <summary>
        /// 歌曲文件名
        /// </summary>
        [ObjectTransferOrder(0, Reverse = false, Offset = 0)]
        public CLUV FileName { get; set; }

        /// <summary>
        /// 歌曲数据流
        /// </summary>
        [ObjectTransferOrder(0, Reverse = false, Offset = -1)]
        public BLV Data { get; set; }
    }
}

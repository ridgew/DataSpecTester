using System;
using System.Collections.Generic;
using System.Text;
using EaseServer.Configuration;

namespace EaseServer.ESP
{
    /// <summary>
    /// 歌词结构
    /// </summary>
    public class LYRIC : ESPDataBase
    {
        public LYRIC(ESPContext context)
            : base(context)
        { 
        
        }

        /// <summary>
        /// 歌词文件名
        /// </summary>
        [ObjectTransferOrder(0, Reverse = false, Offset = 0)]
        public CLUV FileName { get; set; }

        /// <summary>
        /// 歌词内容
        /// </summary>
        [ObjectTransferOrder(1, Reverse = false, Offset = -1)]
        public CLUV FileContent { get; set; }

    }
}

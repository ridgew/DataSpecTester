using System;
using System.Collections.Generic;
using System.Text;
using Gwsoft.DataSpec;

namespace Gwsoft.EaseMode
{
    /// <summary>
    /// 分包数据响应
    /// </summary>
    public abstract class PackageResponse : ESPDataBase
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="PackageResponse"/> class.
        /// </summary>
        public PackageResponse()
            : base()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PackageResponse"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public PackageResponse(ESPContext context)
            : base(context)
        { }


        #region 属性模型顺序
        /// <summary>
        /// 响应头信息
        /// </summary>
        [ObjectTransferOrder(0, Reverse = false, Offset = 0)]
        public ResponseHeader ESP_Header { get; set; }

        /// <summary>
        /// 服务器端响应码(响应码不为0时弹出提示框)
        /// </summary>
        [ObjectTransferOrder(1, Reverse = true, Offset = -1)]
        public StatusCode ESP_Code { get; set; }

        /// <summary>
        /// 提示信息
        /// </summary>
        [ObjectTransferOrder(2, Reverse = false, Offset = 2)]
        public EaseString ESP_Message { get; set; }

        /// <summary>
        /// 请求的包序号(分包首个包序号为1)
        /// </summary>
        [ObjectTransferOrder(3, Reverse = true, Offset = -1)]
        public short ESP_PackageIndex { get; set; }

        /// <summary>
        /// 后续请求包的个数
        /// </summary>
        [ObjectTransferOrder(4, Reverse = true, Offset = 2)]
        public short ESP_LeavePackageCount { get; set; }

        /// <summary>
        /// 本次分包返回的数据长度
        /// </summary>
        [ObjectTransferOrder(5, Reverse = true, Offset = 2)]
        public int ESP_PackageLength { get; set; }

        /// <summary>
        /// 分包数据
        /// </summary>
        [ObjectTransferOrder(6, Reverse = false, Offset = 4)]
        public byte[] ESP_PackageData { get; set; }
        #endregion


        ///// <summary>
        ///// 作为自身基类的相关属性绑定
        ///// </summary>
        ///// <typeparam name="TEntity">The type of the entity.</typeparam>
        //protected BindBuilder SubClassPropertyBindAction<TEntity>()
        //    where TEntity : PackageResponse
        //{
        //    return BindBuilder.Instance()
        //            .Add((TEntity resp) => resp.ESP_PackageData,
        //                (s, obj) =>
        //                {
        //                    TEntity cResp = (TEntity)obj;
        //                    cResp.ESP_PackageData = s.ReadNetworkStreamBytes(cResp.ESP_PackageLength);
        //                    return cResp.ESP_PackageData;
        //                });
        //}
    }
}

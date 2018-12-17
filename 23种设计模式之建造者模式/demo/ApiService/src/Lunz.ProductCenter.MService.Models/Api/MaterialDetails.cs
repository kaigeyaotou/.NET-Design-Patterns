using System;
using System.Collections.Generic;
using System.Text;

namespace Lunz.ProductCenter.MService.Models.Api
{
    /// <summary>
    /// 物料
    /// </summary>
    public class MaterialDetails : Core.Models.BasicProducts.MaterialDetails
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 编号
        /// </summary>
        public string MateCode { get; set; }

        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool IsDisable { get; set; }
    }
}

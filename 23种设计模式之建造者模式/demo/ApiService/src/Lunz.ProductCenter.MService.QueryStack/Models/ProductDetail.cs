using System;
using System.Collections.Generic;
using System.Text;

namespace Lunz.ProductCenter.MService.QueryStack.Models
{
    /// <summary>
    /// 产品-物料明细
    /// </summary>
    public class ProductDetail
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 产品 Id
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// 物料 Id
        /// </summary>
        public string MaterialId { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int? Quantity { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool? Deleted { get; set; }

        /// <summary>
        /// 删除时间
        /// </summary>
        public DateTime? DeletedAt { get; set; }
    }
}

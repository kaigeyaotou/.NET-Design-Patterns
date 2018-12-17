using System;
using System.Collections.Generic;
using System.Text;

namespace Lunz.ProductCenter.MService.QueryStack.Models
{
    /// <summary>
    /// 物料采购主体
    /// </summary>
    public class Trade
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 物料 Id
        /// </summary>
        public string ProdMaterId { get; set; }

        /// <summary>
        /// 采购主体 Id
        /// </summary>
        public string TradeId { get; set; }

        /// <summary>
        /// 采购主体名称
        /// </summary>
        public string TradeName { get; set; }

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

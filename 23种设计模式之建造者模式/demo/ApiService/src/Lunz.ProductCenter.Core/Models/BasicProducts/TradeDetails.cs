using System;
using System.Collections.Generic;
using System.Text;

namespace Lunz.ProductCenter.Core.Models.BasicProducts
{
    /// <summary>
    /// 物料采购主体
    /// </summary>
    public class TradeDetails
    {
        /// <summary>
        /// 采购主体 Id
        /// </summary>
        public string TradeId { get; set; }

        /// <summary>
        /// 采购主体名称
        /// </summary>
        public string TradeName { get; set; }
    }
}

using System.Collections.Generic;
using System.Linq;

namespace Lunz.ProductCenter.ApiService.Trade.Api
{
    public class TradeDetails
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 关联的产品 Id
        /// </summary>
        public string ProdMaterId { get; set; }

        /// <summary>
        /// 采购主体Id
        /// </summary>
        public string TradeId { get; set; }

        /// <summary>
        /// 采购主体名称
        /// </summary>
        public string TradeName { get; set; }

        public string TradeId2TradeName
        {
            get
            {
                return $"{TradeId}|||{TradeName}";
            }
        }
    }
}

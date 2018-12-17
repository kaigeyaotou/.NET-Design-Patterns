using System;

namespace Lunz.ProductCenter.ApiService.ProductInfo.Api
{
    public class ProductInfoDetails
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 产品Code
        /// </summary>
        public string ProdCode { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProdName { get; set; }

        /// <summary>
        /// 产品全称
        /// </summary>
        public string ProdFullName { get; set; }

        /// <summary>
        /// 产品类型Id
        /// </summary>
        public string ProdTypeId { get; set; }

        /// <summary>
        /// 产品类型名称
        /// </summary>
        public string ProdTypeName { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public decimal? Price { get; set; }

        /// <summary>
        /// 审核状态
        /// </summary>
        public int? AuditState { get; set; }

        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime AuditTime { get; set; }

        /// <summary>
        /// 销售渠道
        /// </summary>
        public string TradeName { get; set; }

        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool IsDisable { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool Deleted { get; set; }
    }
}

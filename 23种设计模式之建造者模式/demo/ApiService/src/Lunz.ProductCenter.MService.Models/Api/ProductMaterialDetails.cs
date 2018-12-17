using System;

namespace Lunz.ProductCenter.ApiService.ProductDetail.Api
{
    public class ProductMaterialDetails
    {
        /// <summary>
        /// 产品-物料关系主键
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 产品Id
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// 物料Id
        /// </summary>
        public string MaterialId { get; set; }

        /// <summary>
        /// 物料code
        /// </summary>
        public string MateCode { get; set; }

        /// <summary>
        /// 物料名称
        /// </summary>
        public string MateName { get; set; }

        /// <summary>
        /// 关联物料的属性集合
        /// </summary>
        public string PropName { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int? Quantity { get; set; }

        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool IsDisable { get; set; }
    }
}

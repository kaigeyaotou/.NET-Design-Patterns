using System;
using System.Collections.Generic;
using System.ComponentModel;
using Lunz.Domain.Kernel.Repositories;

namespace Lunz.ProductCenter.ApiService.ProductVehicle.Api
{
    public class ProductVehicleDetails
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 关联产品id
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// 品牌名称
        /// </summary>
        public string BrandName { get; set; }

        /// <summary>
        /// 子品牌名称
        /// </summary>
        public string ChildBrandName { get; set; }

        /// <summary>
        /// 车系名称
        /// </summary>
        public string SeriesName { get; set; }

        /// <summary>
        /// 车型名称
        /// </summary>
        public string ModelName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public string UpdatedAt { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool? Deleted { get; set; }

        /// <summary>
        /// 删除时间
        /// </summary>
        public DateTime DeletedAt { get; set; }
    }
}

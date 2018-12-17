using System;
using System.Collections.Generic;
using System.ComponentModel;
using Lunz.Domain.Kernel.Repositories;

namespace Lunz.ProductCenter.ApiService.QueryStack.Models
{
    public class ProductVehicle
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 数据中心车型主键
        /// </summary>
        public string VehicleId { get; set; }

        /// <summary>
        /// 关联产品id
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// 品牌Id
        /// </summary>
        public string BrandId { get; set; }

        /// <summary>
        /// 品牌名称
        /// </summary>
        public string BrandName { get; set; }

        /// <summary>
        /// 子品牌Id
        /// </summary>
        public string ChildBrandId { get; set; }

        /// <summary>
        /// 子品牌名称
        /// </summary>
        public string ChildBrandName { get; set; }

        /// <summary>
        /// 车系Id
        /// </summary>
        public string SeriesId { get; set; }

        /// <summary>
        /// 车系名称
        /// </summary>
        public string SeriesName { get; set; }

        /// <summary>
        /// 车型Id
        /// </summary>
        public string ModelId { get; set; }

        /// <summary>
        /// 车型名称
        /// </summary>
        public string ModelName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// 年份
        /// </summary>
        public int? Year { get; set; }

        /// <summary>
        /// 排量
        /// </summary>
        public decimal? Displacement { get; set; }

        /// <summary>
        /// 排量显示名称
        /// </summary>
        public string DisplacementText { get; set; }

        /// <summary>
        /// 马力
        /// </summary>
        public int? Horsepower { get; set; }

        /// <summary>
        /// 进气形式
        /// </summary>
        public string IntakeType { get; set; }

        /// <summary>
        /// 变速箱
        /// </summary>
        public string GearboxName { get; set; }

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

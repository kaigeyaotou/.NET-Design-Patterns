using System;
using System.Collections.Generic;
using System.Text;
using Lunz.Domain.Kernel.Repositories;

namespace Lunz.ProductCenter.MService.QueryStack.Models
{
    /// <summary>
    /// 物料
    /// </summary>
    public class Material : IEntity<string>
    {
        public Material()
        {
            Properties = new List<MaterialProperty>();
            Trades = new List<Trade>();
        }

        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 物料编码
        /// </summary>
        public string MateCode { get; set; }

        /// <summary>
        /// 物料名称
        /// </summary>
        public string MateName { get; set; }

        /// <summary>
        /// 物料类型 Id
        /// </summary>
        public string MateTypeId { get; set; }

        /// <summary>
        /// 物料类型编码
        /// </summary>
        public string MateTypeCode { get; set; }

        /// <summary>
        /// 物料类型名称
        /// </summary>
        public string MateTypeName { get; set; }

        /// <summary>
        /// 物料规格 Id
        /// </summary>
        public string MaterialSpecId { get; set; }

        /// <summary>
        /// 物料规格
        /// </summary>
        public string MaterialSpec { get; set; }

        /// <summary>
        /// 物料单位 Id
        /// </summary>
        public string MaterialUnitId { get; set; }

        /// <summary>
        /// 物料单位
        /// </summary>
        public string MaterialUnits { get; set; }

        /// <summary>
        /// 物料参考采购价
        /// </summary>
        public decimal MaterialPrice { get; set; }

        /// <summary>
        /// 是否有独立编码
        /// </summary>
        public bool IdCodeSingle { get; set; }

        /// <summary>
        /// 是否为生产物料
        /// </summary>
        public bool IsSelfBuild { get; set; }

        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool IsDisable { get; set; }

        /// <summary>
        /// 物料属性
        /// </summary>
        public List<MaterialProperty> Properties { get; set; }

        /// <summary>
        /// 物料采购主体
        /// </summary>
        public List<Trade> Trades { get; set; }
    }
}

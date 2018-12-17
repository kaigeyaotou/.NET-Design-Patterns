using System;
using System.Collections.Generic;
using System.Text;

namespace Lunz.ProductCenter.Core.Models.BasicProducts
{
    /// <summary>
    /// 物料
    /// </summary>
    public class MaterialDetails
    {
        public MaterialDetails()
        {
            Properties = new List<MaterialPropertyDetails>();
            Trades = new List<TradeDetails>();
        }

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
        /// 物料属性
        /// </summary>
        public List<MaterialPropertyDetails> Properties { get; set; }

        /// <summary>
        /// 物料采购主体
        /// </summary>
        public List<TradeDetails> Trades { get; set; }
    }
}

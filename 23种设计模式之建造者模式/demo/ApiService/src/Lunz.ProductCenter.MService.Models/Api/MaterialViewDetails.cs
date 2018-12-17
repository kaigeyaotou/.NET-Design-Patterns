using System;
using System.Collections.Generic;
using System.Text;

namespace Lunz.ProductCenter.MService.Models.Api
{
    public class MaterialViewDetails
    {
        /// <summary>
        /// 物料 Id
        /// </summary>
        public string Id
        {
            get
            {
                return MaterialId;
            }
        }

        /// <summary>
        /// 物料 Id
        /// </summary>
        public string MaterialId { get; set; }

        /// <summary>
        /// 物料编码
        /// </summary>
        public string MateCode { get; set; }

        /// <summary>
        /// 物料名称
        /// </summary>
        public string MateName { get; set; }

        /// <summary>
        /// 物料类型名称
        /// </summary>
        public string MateTypeName { get; set; }

        /// <summary>
        /// 物料类型编码
        /// </summary>
        public string MateTypeCode { get; set; }

        /// <summary>
        /// 物料规格
        /// </summary>
        public string MaterialSpec { get; set; }

        /// <summary>
        /// 是否为生产物料
        /// </summary>
        public bool IsSelfBuild { get; set; }

        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool IsDisable { get; set; }

        /// <summary>
        /// 是否发布
        /// </summary>
        public bool AuditState { get; set; }

        /// <summary>
        /// 物料属性名字符串集合
        /// </summary>
        public string PropName { get; set; }

        /// <summary>
        /// 采购主体集合
        /// </summary>
        public string TradeName { get; set; }

        /// <summary>
        /// 建议采购价
        /// </summary>
        public decimal MaterialPrice { get; set; }
    }
}

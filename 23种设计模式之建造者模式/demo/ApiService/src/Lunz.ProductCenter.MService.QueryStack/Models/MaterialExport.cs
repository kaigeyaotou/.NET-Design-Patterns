using System;
using System.Collections.Generic;
using Lunz.Domain.Kernel.Repositories;

namespace Lunz.ProductCenter.MService.QueryStack.Models
{
    /// <summary>
    /// 物料
    /// </summary>
    public class MaterialExport : IEntity<string>
    {
        public MaterialExport()
        {
            Properties = new List<MaterialPropertyExport>();
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
        /// 物料类型名称
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// 物料规格
        /// </summary>
        public string MaterialSpec { get; set; }

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
        /// 物料审核状态, 0:提交, 1:审核通过, 2:拒绝
        /// </summary>
        public int AuditState { get; set; }

        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime AuditTime { get; set; }

        /// <summary>
        /// 物料采购主体
        /// </summary>
        public string TradeName { get; set; }

        /// <summary>
        /// 是否启用，0:启用，1:禁用
        /// </summary>
        public bool IsDisable { get; set; }

        /// <summary>
        /// 物料属性
        /// </summary>
        public List<MaterialPropertyExport> Properties { get; set; }
    }
}

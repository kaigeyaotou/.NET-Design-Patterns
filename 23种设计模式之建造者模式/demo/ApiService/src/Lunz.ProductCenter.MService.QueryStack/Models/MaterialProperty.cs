using System;
using System.Collections.Generic;
using System.Text;

namespace Lunz.ProductCenter.MService.QueryStack.Models
{
    /// <summary>
    /// 物料属性
    /// </summary>
    public class MaterialProperty
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 物料 Id
        /// </summary>
        public string MaterialId { get; set; }

        /// <summary>
        /// 属性 Id
        /// </summary>
        public string PropId { get; set; }

        /// <summary>
        /// 属性名称
        /// </summary>
        public string PropName { get; set; }

        /// <summary>
        /// 属性值 Id
        /// </summary>
        public string OptionId { get; set; }
    }
}

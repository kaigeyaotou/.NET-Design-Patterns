using System;
using System.Collections.Generic;
using System.Text;

namespace Lunz.ProductCenter.Core.Models.BasicProducts
{
    /// <summary>
    /// 物料属性
    /// </summary>
    public class MaterialPropertyDetails
    {
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

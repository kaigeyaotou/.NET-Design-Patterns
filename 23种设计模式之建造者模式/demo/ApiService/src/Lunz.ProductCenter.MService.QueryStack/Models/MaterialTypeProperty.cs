using System;
using System.Collections.Generic;

namespace Lunz.ProductCenter.MService.QueryStack.Models
{
    /// <summary>
    /// 物料类型属性
    /// </summary>
    public class MaterialTypeProperty
    {
        public MaterialTypeProperty()
        {
            Options = new List<PropertyOption>();
        }

        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 物料类型 Id
        /// </summary>
        public string MateTypeId { get; set; }

        /// <summary>
        /// 属性 Id
        /// </summary>
        public string PropId { get; set; }

        /// <summary>
        /// 属性名称
        /// </summary>
        public string PropName { get; set; }

        /// <summary>
        /// 属性显示名称
        /// </summary>
        public string PropDisplayName { get; set; }

        /// <summary>
        /// 属性创建时间
        /// </summary>
        public DateTime PropCreatedAt { get; set; }

        /// <summary>
        /// 是否必填
        /// </summary>
        public bool IsNecessary { get; set; }

        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool IsDisable { get; set; }

        /// <summary>
        /// 属性值
        /// </summary>
        public List<PropertyOption> Options { get; set; }

        /// <summary>
        /// 属性创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 属性更新时间
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}

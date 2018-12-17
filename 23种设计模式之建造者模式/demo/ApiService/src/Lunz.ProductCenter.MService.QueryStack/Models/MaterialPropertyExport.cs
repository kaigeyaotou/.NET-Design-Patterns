using System.Collections.Generic;

namespace Lunz.ProductCenter.MService.QueryStack.Models
{
    /// <summary>
    /// 物料属性
    /// </summary>
    public class MaterialPropertyExport
    {
        public MaterialPropertyExport()
        {
            Options = new List<PropertyOptionExport>();
        }

        /// <summary>
        /// 主键
        /// </summary>
        public string PropId { get; set; }

        /// <summary>
        /// 属性显示名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 是否必填，0:否，1:是
        /// </summary>
        public bool IsNecessary { get; set; }

        /// <summary>
        /// 属性值
        /// </summary>
        public string OptionName { get; set; }

        /// <summary>
        /// 属性值
        /// </summary>
        public List<PropertyOptionExport> Options { get; set; }
    }
}

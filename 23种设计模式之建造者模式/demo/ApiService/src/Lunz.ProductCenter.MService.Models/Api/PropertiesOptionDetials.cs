using System.Collections.Generic;
using System.Linq;

namespace Lunz.ProductCenter.ApiService.PropertiesOption.Api
{
    public class PropertiesOptionDetials
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 关联属性id
        /// </summary>
        public string PropId { get; set; }

        /// <summary>
        /// 选项名称
        /// </summary>
        public string OptionName { get; set; }

        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool IsDisable { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public string IsEnableDescription
        {
            get { return !IsDisable ? "禁用" : "启用"; }
        }

        /// <summary>
        /// 排序权重
        /// </summary>
        public int? SortOrder { get; set; }
    }
}

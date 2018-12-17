using System.Collections.Generic;
using System.Linq;

namespace Lunz.ProductCenter.ApiService.Property.Api
{
    public class PropertyDetails
    {
        /// <summary>
        /// 属性Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 属性名称
        /// </summary>
        public string PropName { get; set; }

        /// <summary>
        /// 属性显示名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool IsDisable { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public string IsDisableDescription
        {
            get
            {
                return !IsDisable ? "启用" : "禁用";
            }
        }

        /// <summary>
        /// 是否启用
        /// </summary>
        public string IsEnableDescription
        {
            get
            {
                return IsDisable ? "启用" : "禁用";
            }
        }
    }
}

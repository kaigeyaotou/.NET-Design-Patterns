using System;
using System.Collections.Generic;
using System.Linq;
using Lunz.Domain.Kernel.Repositories;

namespace Lunz.ProductCenter.ApiService.QueryStack.Models
{
    public class Property
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 属性名称
        /// </summary>
        public string PropName { get; set; }

        /// <summary>
        /// 属性code
        /// </summary>
        public string PropCode { get; set; }

        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool? IsDisable { get; set; }

        /// <summary>
        /// 属性显示名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool? Deleted { get; set; }

        /// <summary>
        /// 删除时间
        /// </summary>
        public DateTime? DeletedAt { get; set; }
    }
}

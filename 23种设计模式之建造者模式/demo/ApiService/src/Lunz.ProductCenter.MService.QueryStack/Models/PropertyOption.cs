using System;
using System.Collections.Generic;
using System.Text;

namespace Lunz.ProductCenter.MService.QueryStack.Models
{
    /// <summary>
    /// 属性值
    /// </summary>
    public class PropertyOption
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
        /// 排序字段
        /// </summary>
        public int? SortOrder { get; set; }

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

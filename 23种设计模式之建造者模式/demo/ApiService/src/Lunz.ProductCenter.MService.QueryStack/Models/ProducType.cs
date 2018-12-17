using System;
using System.Collections.Generic;
using System.Linq;
using Lunz.Domain.Kernel.Repositories;
using Lunz.ProductCenter.MService.QueryStack.Models;

namespace Lunz.ProductCenter.ApiService.QueryStack.Models
{
    public class ProducType : MoveBaseModel
    {
        public ProducType()
        {
            Children = new List<ProducType>();
        }
        ///// <summary>
        ///// 主键
        ///// </summary>
        // public string Id { get; set; }

        /// <summary>
        /// 父级Id
        /// </summary>
        // public string ParentId { get; set; }

        /// <summary>
        /// 级别
        /// </summary>
        // public int? LevelCode { get; set; }

        /// <summary>
        /// 类型名称
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// 类型编码
        /// </summary>
        public string TypeCode { get; set; }

        ///// <summary>
        ///// 属性显示名称
        ///// </summary>
        // public int? SortOrder { get; set; }

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


        public List<ProducType> Children { get; set; }
    }
}

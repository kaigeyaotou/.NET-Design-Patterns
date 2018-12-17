using System;
using System.Collections.Generic;
using System.Text;

namespace Lunz.ProductCenter.MService.Models.Api
{
    /// <summary>
    /// 物料类型
    /// </summary>
    public class MaterialTypeDetails
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// ParentId
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        public string TypeCode { get; set; }

        /// <summary>
        /// 等级
        /// </summary>
        public int LevelCode { get; set; }

        /// <summary>
        /// 是否有子级
        /// </summary>
        public bool Children { get; set; }
    }
}

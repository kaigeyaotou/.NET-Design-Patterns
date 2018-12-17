using System;
using System.Collections.Generic;
using System.Text;

namespace Lunz.ProductCenter.Core.Models.BasicProducts
{
    /// <summary>
    /// 物料类型
    /// </summary>
    public class MaterialTypeDetails
    {
        /// <summary>
        /// 父级 Id
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// 物料类型名称
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// 物料类型等级
        /// </summary>
        public int LevelCode { get; set; }
    }
}

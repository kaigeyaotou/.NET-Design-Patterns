using System.Collections.Generic;

namespace Lunz.ProductCenter.MService.Models.Api
{
    /// <summary>
    /// 物料类型树
    /// </summary>
    public class MaterialTypeTreeDetails
    {
        public MaterialTypeTreeDetails()
        {
            TreeChildren = new List<MaterialTypeTreeDetails>();
        }

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
        public bool HasChild { get; set; }

        /// <summary>
        /// 子级集合
        /// </summary>
        public List<MaterialTypeTreeDetails> TreeChildren { get; set; }
    }
}

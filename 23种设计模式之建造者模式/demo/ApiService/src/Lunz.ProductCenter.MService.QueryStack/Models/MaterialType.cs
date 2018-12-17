using Lunz.Domain.Kernel.Repositories;

namespace Lunz.ProductCenter.MService.QueryStack.Models
{
    /// <summary>
    /// 物料类型
    /// </summary>
    public class MaterialType : MoveBaseModel, IEntity<string>
    {
        ///// <summary>
        ///// Id
        ///// </summary>
        // public string Id { get; set; }

        /// <summary>
        /// 父级 Id
        /// </summary>
        // public string ParentId { get; set; }

        /// <summary>
        /// 物料类型 Code
        /// </summary>
        public string TypeCode { get; set; }

        /// <summary>
        /// 物料类型名称
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// 物料类型等级
        /// </summary>
        // public int LevelCode { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        // public int SortOrder { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool Deleted { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreatedById { get; set; }
    }
}

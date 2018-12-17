using System.Collections.Generic;
using Lunz.Domain.Kernel.Repositories;

namespace Lunz.ProductCenter.MService.QueryStack.Models
{
    /// <summary>
    /// 物料类型
    /// </summary>
    public class MaterialTypeExport : IEntity<string>
    {
        public MaterialTypeExport()
        {
            Properties = new List<MaterialPropertyExport>();
        }

        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 物料类型名称
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// 物料属性
        /// </summary>
        public List<MaterialPropertyExport> Properties { get; set; }
    }
}

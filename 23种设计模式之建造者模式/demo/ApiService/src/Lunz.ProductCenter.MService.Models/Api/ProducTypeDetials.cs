using System.Collections.Generic;
using Lunz.ProductCenter.MService.QueryStack.Models;

namespace Lunz.ProductCenter.ApiService.ProducType.Api
{
    public class ProducTypeDetials : MoveBaseModel
    {
        public ProducTypeDetials()
        {
            Children = new List<ProducTypeDetials>();
        }

        ///// <summary>
        ///// 主键
        ///// </summary>
        // public string Id { get; set; }

        ///// <summary>
        ///// 父级Id
        ///// </summary>
        // public string ParentId { get; set; }

        ///// <summary>
        ///// 级别
        ///// </summary>
        // public int? LevelCode { get; set; }

        /// <summary>
        /// 类型名称
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// 类型编码
        /// </summary>
        public string TypeCode { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        // public int? SortOrder { get; set; }
        // public bool HasChildren { get; set; }
        public List<ProducTypeDetials> Children { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Lunz.ProductCenter.MService.QueryStack.Models
{
    /// <summary>
    /// 数据字典
    /// </summary>
    public class DataDictionary
    {
        public DataDictionary()
        {
            Children = new List<DataDictionary>();
        }

        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 排序字段
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// 子级
        /// </summary>
        public List<DataDictionary> Children { get; set; }
    }
}

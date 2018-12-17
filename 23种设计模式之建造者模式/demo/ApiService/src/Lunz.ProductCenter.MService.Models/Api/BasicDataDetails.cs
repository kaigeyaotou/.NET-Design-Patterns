using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Lunz.ProductCenter.ApiService.BasicData.Api
{
    public class BasicDataDetails
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        public string Id2Name
        {
            get
            {
                return $"{Id}|||{Name}";
            }
        }

        /// <summary>
        /// 父级id
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public string SortOrder { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool? Enabled { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreatedAtFormater
        {
            get
            {
                if (CreatedAt.HasValue)
                {
                    return CreatedAt.Value.ToString();
                }

                return string.Empty;
            }
        }

        public DateTime? CreatedAt { get; set; }
    }
}

﻿using System.Collections.Generic;

namespace Lunz.ProductCenter.Core.Models
{
    /// <summary>
    /// 具有分页的数据列表
    /// </summary>
    /// <typeparam name="T">数据实体</typeparam>
    public interface IPaginationModel<T>
    {
        /// <summary>
        /// 总行数
        /// </summary>
        long Count { get; }

        /// <summary>
        /// 当前页号
        /// </summary>
        int? PageIndex { get; }

        /// <summary>
        /// 每页行数
        /// </summary>
        int? PageSize { get; }

        /// <summary>
        /// 数据列表
        /// </summary>
        IEnumerable<T> Data { get; }
    }
}

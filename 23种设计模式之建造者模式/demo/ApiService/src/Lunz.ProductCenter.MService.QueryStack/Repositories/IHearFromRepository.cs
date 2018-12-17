﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lunz.Domain.Kernel.Repositories;
using Lunz.ProductCenter.ApiService.QueryStack.Models;

namespace Lunz.ProductCenter.ApiService.QueryStack.Repositories
{
    public interface IHearFromRepository : IRepository<Guid, HearFrom>
    {
        Task<(long Count, IEnumerable<HearFrom> Data)> QueryAsync(
            Func<(string Sql, dynamic Parameters)> filter = null,
            int? pageIndex = null,
            int? pageSize = null,
            string orderBy = null,
            bool hasCountResult = false);

        Task<(long Count, IEnumerable<T> Data)> QueryAsync<T>(
            Func<(string Sql, dynamic Parameters)> filter = null,
            int? pageIndex = null,
            int? pageSize = null,
            string orderBy = null,
            bool hasCountResult = false);
    }
}
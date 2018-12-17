using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lunz.Domain.Kernel.Repositories;
using Lunz.ProductCenter.ApiService.QueryStack.Models;

namespace Lunz.ProductCenter.ApiService.QueryStack.Repositories
{
    public interface IPropertyRespository
    {
        Task AddAsync(Property entity, string userId);

        Task DisableAsync(string id, string userId);

        Task EnableAsync(string id, string userId);

        Task UpdateAsync(Property entity, string userId);

        Task<Property> FindAsync(string key);

        Task<Property> FindPropertyAsync(string key);

        Task<bool> CheckdPropNameAsync(string propname, string excludeId = "");

        Task<(long Count, IEnumerable<Property> Data)> QueryAsync(
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

        Task<(long Count, IEnumerable<T> Data)> QueryRealityAsync<T>(
            Func<(string Sql, dynamic Parameters)> filter = null,
            int? pageIndex = null,
            int? pageSize = null,
            string orderBy = null,
            bool hasCountResult = false);
    }
}

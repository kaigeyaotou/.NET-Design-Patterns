using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lunz.Domain.Kernel.Repositories;
using Lunz.ProductCenter.ApiService.QueryStack.Models;

namespace Lunz.ProductCenter.ApiService.QueryStack.Repositories
{
    public interface IProducTypeRepository
    {
        Task AddAsync(ApiService.QueryStack.Models.ProducType entity, string userId);

        Task UpdateAsync(QueryStack.Models.ProducType entity, string userId);

        Task<QueryStack.Models.ProducType> FindAsync(string id);

        Task<bool> CheckTheTopTypeName(string typeName, string excludeId = "");

        Task<bool> CheckTheSameTypeName(string typeName, string parentId, string excludeId = "");

        Task<int> GetMaxSortOrderValue(string parentId);

        Task<string> GetMaxCode(string parentId);

        Task<bool> HasChildren(string parentId, string typeName = "");

        Task<IEnumerable<T>> QueryAsync<T>(string parentId = "");

        Task<IEnumerable<T>> SearchByTypeName<T>(string typeName);
    }
}

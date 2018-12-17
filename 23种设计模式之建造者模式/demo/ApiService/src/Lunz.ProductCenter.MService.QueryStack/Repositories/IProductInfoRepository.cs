using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lunz.ProductCenter.ApiService.QueryStack.Models;

namespace Lunz.ProductCenter.ApiService.QueryStack.Repositories
{
    public interface IProductInfoRepository
    {
        Task<(long Count, IEnumerable<ProductInfo> Data)> QueryAsync(
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

        Task DisableAsync(List<string> ids, string userId);

        Task EnableAsync(List<string> ids, string userId);

        Task DeleteAsync(string id, string userId);

        Task<bool> IsReleaseAsync(string id);

        Task ReleaseAsync(string[] ids, string userId);

        Task AddAsync(ProductInfo entity, string userId);

        Task UpdateAsync(ProductInfo entity, string userId);

        Task<ProductInfo> DetailAsync(string id);

        Task<ProductInfo> FindAsync(string id);

        Task<ProductInfo> FindByProductFullNameAsync(string productFullName);

        Task<string> GetMaxProductCodeAsync();

        Task<bool> HasProductFullNameAsync(string productFullName, string excludeId = "");

        Task<bool> HasProductOfThisTypeAsync(string productTypeId);
    }
}

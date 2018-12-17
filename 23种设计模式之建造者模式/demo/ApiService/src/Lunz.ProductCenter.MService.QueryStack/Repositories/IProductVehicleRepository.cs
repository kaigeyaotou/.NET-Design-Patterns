using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lunz.Domain.Kernel.Repositories;
using Lunz.ProductCenter.ApiService.QueryStack.Models;

namespace Lunz.ProductCenter.ApiService.QueryStack.Repositories
{
    public interface IProductVehicleRepository
    {
        Task AddAsync(List<QueryStack.Models.ProductVehicle> entities);

        Task DeleteAsync(string id, string userId);

        Task<bool> HasSameModel(string productId, string vehicleId);

        Task<bool> HasSameSeries(string productId, string seriesId);

        Task<IEnumerable<T>> QueryByProductId<T>(string productId);

        Task<(long Count, IEnumerable<ProductVehicle> Data)> QueryAsync(
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

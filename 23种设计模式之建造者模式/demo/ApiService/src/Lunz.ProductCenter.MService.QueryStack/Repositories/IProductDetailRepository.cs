using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lunz.Domain.Kernel.Repositories;
using Lunz.ProductCenter.ApiService.QueryStack.Models;
using Lunz.ProductCenter.MService.QueryStack.Models;

namespace Lunz.ProductCenter.ApiService.QueryStack.Repositories
{
    public interface IProductDetailRepository
    {
        Task AddAsync(ProductDetail entity, string userId);

        Task UpdateAsync(ProductDetail entity, string userId);

        Task DeleteAsync(string[] ids, string userId);

        Task DeleteByProductIdAsync(string productId, string userId);

        Task<IEnumerable<T>> QueryAsync<T>(string productId);
    }
}

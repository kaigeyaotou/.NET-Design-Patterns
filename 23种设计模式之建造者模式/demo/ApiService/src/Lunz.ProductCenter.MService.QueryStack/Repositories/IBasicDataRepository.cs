using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lunz.Domain.Kernel.Repositories;
using Lunz.ProductCenter.ApiService.QueryStack.Models;

namespace Lunz.ProductCenter.ApiService.QueryStack.Repositories
{
    public interface IBasicDataRepository
    {
        Task<IEnumerable<T>> QueryByParentIdAsync<T>(string parentId);

        Task<(long Count, IEnumerable<T> Data)> QueryAsync<T>(
            Func<(string Sql, dynamic Parameters)> filter = null,
            int? pageIndex = null,
            int? pageSize = null,
            string orderBy = null,
            bool hasCountResult = false);

        Task<bool> HasSameChildrenNameAsync(string parentId, string name, string thisId = "");

        Task<BasicData> FindAsync(string id);

        Task<bool> AllowDisabledOfThisOptionAsync(string parentId, string thisId = "");

        //Task<Property> FindAsync(string id);

        Task<int> GetMaxSortOrderValue(string parentId);

        Task<string> GetMaxCodeValue(string parentId);

        Task AddAsync(BasicData entity, string userId);

        Task UpdateAsync(BasicData entity, string userId);

        Task UpdateOptionAsync(BasicData entity, string userId);

        Task EnableAsync(string id, string userId);

        Task DisableAsync(string id, string userId);

        Task UpOrDownAsync(BasicData entity, string userId);
    }
}

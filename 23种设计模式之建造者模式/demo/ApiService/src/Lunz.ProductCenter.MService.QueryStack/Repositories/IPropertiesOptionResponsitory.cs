using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lunz.Domain.Kernel.Repositories;
using Lunz.ProductCenter.ApiService.QueryStack.Models;

namespace Lunz.ProductCenter.ApiService.QueryStack.Repositories
{
    public interface IPropertiesOptionResponsitory
    {
        Task AddAsync(PropertiesOption entity, string userId);

        Task EnableAsync(string id, string userId);

        Task DisableAsync(string id, string userId);

        Task UpOrDownAsync(string id, int sortValue, string userId);

        Task UpdateAsync(PropertiesOption entity, string userId);

        Task<PropertiesOption> FindByOptionNameAsync(string propId, string optionName);

        Task<PropertiesOption> FindAsync(string propId);

        Task<int> MaxSortOrderValue(string propId);

        Task<bool> AllowDisabledOfThisOptionAsync(string propertyId, string thisId = "");

        Task<IEnumerable<PropertiesOption>> QueryAsync(string propId);

        Task<IEnumerable<T>> QueryAsync<T>(string propId);
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using Lunz.ProductCenter.MService.QueryStack.Models;

namespace Lunz.ProductCenter.MService.QueryStack.Repositories
{
    public interface IMaterialTypeRepository
    {
        Task<IEnumerable<MaterialType>> AllAsync();

        Task<IEnumerable<MaterialType>> QueryAsync();

        Task<IEnumerable<T>> RootTypesAsync<T>();

        Task<IEnumerable<T>> ChildTypesAsync<T>(string id);

        Task<MaterialType> FindAsync(string id);

        Task<IEnumerable<T>> GetPropertiesAsync<T>(string typeId);

        Task<IEnumerable<MaterialTypeProperty>> GetPropertiesAsync(string typeId);

        Task AddAsync(MaterialType entity);

        Task AddPropertiesAsync(string typeId, string[] propIds, string userId);

        Task SetPropertyStatus(string id, bool isDisable, string userId);

        Task SetPropertyNecessary(string id, bool isNecessary, string userId);

        Task<IEnumerable<T>> SearchByTypeName<T>(string typeName);
    }
}

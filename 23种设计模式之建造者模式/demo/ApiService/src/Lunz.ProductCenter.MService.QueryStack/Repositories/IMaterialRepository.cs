using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lunz.ProductCenter.MService.QueryStack.Models;

namespace Lunz.ProductCenter.MService.QueryStack.Repositories
{
    public interface IMaterialRepository
    {
        Task<Material> FindAsync(string id);

        Task<IEnumerable<Material>> BasicExistAsync<T>(T entity, string id = null);

        Task<(long Count, IEnumerable<T> Data)> QueryAsync<T>(
            string materialTypeId = null,
            Func<(string Sql, dynamic Parameters)> filter = null,
            int? pageIndex = null,
            int? pageSize = null,
            string orderBy = null,
            bool hasCountResult = false);

        Task<(long Count, IEnumerable<T> Data)> QueryForProductAsync<T>(
            string materialTypeId,
            List<string> tradeNames,
            Func<(string Sql, dynamic Parameters)> filter = null,
            int? pageIndex = null,
            int? pageSize = null,
            string orderBy = null,
            bool hasCountResult = false);

        Task AddAsync(Material entity, string userId);

        Task UpdateAsync(Material entity, string userId);

        Task UpdateAsync(string id, decimal materialPrice, string userId);

        Task DeleteAsync(string id, string userId);

        Task<(string Id, Material Data)> GetIdAndMaxCodeAsync();

        Task AddPropertiesAsync(List<MaterialProperty> entities, string userId);

        Task UpdatePropertiesAsync(List<MaterialProperty> entities, string userId);

        Task DeletePropertiesAsync(string[] ids);

        Task AddTradeAsync(List<Trade> entities, string userId);

        Task DeleteTradeAsync(string[] ids);

        Task SetStatusAsync(string[] ids, bool isDisable, string userId);

        Task PublishedAsync(string[] ids, string userId);

        Task<IEnumerable<DataDictionary>> DataDicAsync(bool needAll);

        Task<IEnumerable<MaterialTypeProperty>> GetPropertiesAsync(string materialId);

        Task<IEnumerable<T>> FindMaterialsWithBasicPropertiesByTypeAsync<T>(string typeId);

        Task<IEnumerable<T>> FindMaterialsWithAllPropertiesByTypeAsync<T>(string typeId);

        Task<MaterialTypeExport> FindCustomPropertiesByTypeAsync<T>(string typeId);

        Task<IEnumerable<T>> FindBasicPropertiesAsync<T>();

        Task<IEnumerable<T>> FindAllTradePropertiesAsync<T>();

        Task<bool> IsUploadDateTimeLatestAsync(string typeId, string uploadDateTime);

        Task AddResourceAsync(ResourceItem resource, string userId);

        Task<(long Count, IEnumerable<T> Data)> FindExcelResourcesAsync<T>(
            Func<(string Sql, dynamic Parameters)> filter = null, int? pageIndex = null, int? pageSize = null, string orderBy = null, bool hasCountResult = false);

        Task<IEnumerable<Material>> ExistNameAsync(string name, string materialType, string materialId = "");
    }
}

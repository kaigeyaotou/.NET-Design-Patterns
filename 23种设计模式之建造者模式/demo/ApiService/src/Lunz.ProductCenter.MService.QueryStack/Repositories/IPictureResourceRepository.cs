using System.Collections.Generic;
using System.Threading.Tasks;
using Lunz.ProductCenter.MService.QueryStack.Models;

namespace Lunz.ProductCenter.MService.QueryStack.Repositories
{
    public interface IPictureResourceRepository
    {
        Task AddPictureResourcesAsync(string productId, List<ProductPicture> pictures, string userId);

        Task UpdatePictureResourcesAsync(string productId, List<ProductPicture> pictures, string userId);

        Task DeletePictureResourceAsync(string resourceId, string userId);

        Task<IEnumerable<T>> FindPictureResourcesAsync<T>(string productId);
    }
}

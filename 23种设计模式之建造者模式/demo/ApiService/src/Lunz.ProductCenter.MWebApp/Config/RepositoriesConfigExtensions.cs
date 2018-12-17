using Lunz.ProductCenter.ApiService.QueryStack.MySql.Repositories;
using Lunz.ProductCenter.ApiService.QueryStack.Repositories;
using Lunz.ProductCenter.MService.QueryStack.MySql.Repositories;
using Lunz.ProductCenter.MService.QueryStack.Repositories;
using Lunz.ProductCenter.PropertyManagement.QueryStack.MySql.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Lunz.ProductCenter.WebApp.Config
{
    public static class RepositoriesConfigExtensions
    {
        public static void ConfigureRepositories(this IServiceCollection services)
        {
            services.AddTransient<IHearFromRepository, HearFromRepository>();
            services.AddTransient<IMaterialRepository, MaterialRepository>();
            services.AddTransient<IMaterialTypeRepository, MaterialTypeRepository>();
            services.AddTransient<IPropertyRespository, PropertyRepository>();
            services.AddTransient<IPropertiesOptionResponsitory, PropertiesOptionRepository>();
            services.AddTransient<IProducTypeRepository, ProducTypeRepository>();
            services.AddTransient<IProductInfoRepository, ProductInfoRepository>();
            services.AddTransient<IBasicDataRepository, BasicDataRepository>();
            services.AddTransient<IProductDetailRepository, ProductDetailRepository>();
            services.AddTransient<ITradeRepository, TradeRepository>();
            services.AddTransient<IPictureResourceRepository, PictureResourceRepository>();
            services.AddTransient<IProductVehicleRepository, ProductVehicleRepository>();
            services.AddTransient<IMovePositionRepository, MovePositionRepository>();
        }
    }
}
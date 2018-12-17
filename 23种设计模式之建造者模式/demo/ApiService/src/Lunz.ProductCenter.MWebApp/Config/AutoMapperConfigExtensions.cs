using AutoMapper;
using Lunz.ProductCenter.MService.Api.Material;
using Microsoft.Extensions.DependencyInjection;

namespace Lunz.ProductCenter.WebApp.Config
{
    public static class AutoMapperConfigExtensions
    {
        public static void ConfigureAutoMapper(this IServiceCollection services)
        {
            // INFO: 这里注册更多的 MappingProfile
            var assemblies = new[]
            {
                typeof(MaterialController).Assembly,
            };

            services.AddAutoMapper(assemblies);
        }
    }
}
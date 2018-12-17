using System.Linq;
using FluentValidation.AspNetCore;
using Lunz.ProductCenter.AspNetCore;
using Lunz.ProductCenter.MService.Api.Material;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;

namespace Lunz.ProductCenter.WebApp.Config
{
    public static class MvcConfigExtensions
    {
        public static void ConfigureMvc(this IServiceCollection services)
        {
            // INFO: 在这里配置更多的 Controller 程序集。
            var controllerAssemblies = new[]
            {
                typeof(MaterialController).Assembly,
            };

            // INFO: 这里注册更多的 MediatR's Handler
            services.AddMediatR(controllerAssemblies);

            services.AddMvc(options =>
                {
                    options.Filters.Add<HttpGlobalExceptionFilter>();
                })
                .ConfigureApplicationPartManager(manager =>
                {
                    var feature = new ControllerFeature();

                    // INFO: 在这里注册更多的 Controller 程序集。
                    foreach (var assembly in controllerAssemblies)
                    {
                        manager.ApplicationParts.Add(new AssemblyPart(assembly));
                    }

                    manager.PopulateFeature(feature);
                    services.AddSingleton(feature.Controllers.Select(t => t.AsType()).ToArray());
                })
                .AddFluentValidation(config =>
                {
                    config.ImplicitlyValidateChildProperties = true;
                    foreach (var assembly in controllerAssemblies)
                    {
                        // INFO: 这里注册更多的验证器（Validator）
                        config.RegisterValidatorsFromAssembly(assembly);
                    }
                });

                // .AddMetricsExtensions();
        }

        public static void ConfigureCors(this IApplicationBuilder app)
        {
            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
        }
    }
}
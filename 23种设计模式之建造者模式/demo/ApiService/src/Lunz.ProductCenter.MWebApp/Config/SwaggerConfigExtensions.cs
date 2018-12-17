using Lunz.ProductCenter.MService.Api.Material;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using NJsonSchema;
using NSwag;
using NSwag.AspNetCore;
using NSwag.SwaggerGeneration.Processors.Security;

namespace Lunz.ProductCenter.WebApp.Config
{
    public static class SwaggerConfigExtensions
    {
        public static void UseSwagger(this IApplicationBuilder app, IConfiguration configuration)
        {
            // INFO: 在这里配置更多的 Controller 程序集。
            var controllerAssemblies = new[]
            {
                typeof(MaterialController).Assembly,
            };

            // Enable the Swagger UI middleware and the Swagger generator
            app.UseSwaggerUi3(controllerAssemblies, settings =>
            {
                settings.SwaggerUiRoute = "/docs";
                settings.GeneratorSettings.DefaultPropertyNameHandling = PropertyNameHandling.CamelCase;
                settings.PostProcess = document =>
                {
                    document.Info.Title = "ProductCenter";
                    document.Info.Version = Program.Version;
                };

                settings.OAuth2Client = new OAuth2ClientSettings
                {
                    ClientId = "pdcenter-client-dev",
                    ClientSecret = "LunzProductCenter",
                    AppName = "ProductCenter",
                };

                settings.GeneratorSettings.DocumentProcessors
                    .Add(new SecurityDefinitionAppender("oauth2", new SwaggerSecurityScheme
                    {
                        Type = SwaggerSecuritySchemeType.OAuth2,
                        Description = string.Empty,
                        Flow = SwaggerOAuth2Flow.Password,
                        AuthorizationUrl = $"{configuration["Identity:Authority"]}/account/login",
                        TokenUrl = $"{configuration["Identity:Authority"]}/connect/token",
                    }));

                settings.GeneratorSettings.OperationProcessors
                    .Add(new OperationSecurityScopeProcessor("oauth2"));
            });

            app.UseSwaggerReDoc(controllerAssemblies, settings =>
            {
                settings.SwaggerUiRoute = "/redoc";
                settings.GeneratorSettings.DefaultPropertyNameHandling = PropertyNameHandling.CamelCase;
                settings.PostProcess = document =>
                {
                    document.Info.Title = "ProductCenter";
                    document.Info.Version = Program.Version;
                };
            });
        }
    }
}
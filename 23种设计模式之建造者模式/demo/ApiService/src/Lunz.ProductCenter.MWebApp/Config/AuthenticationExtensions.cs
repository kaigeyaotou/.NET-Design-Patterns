using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lunz.ProductCenter.WebApp.Config
{
    public static class AuthenticationExtensions
    {
        public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = configuration["Identity:Authority"];
                    options.RequireHttpsMetadata = configuration["Identity:EnableSSL"].Equals("1");
                    options.TokenValidationParameters.ValidateAudience = false;
                });
        }
    }
}

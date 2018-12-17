using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lunz.ProductCenter.Health.Health;
using Lunz.ProductCenter.Health.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace Lunz.ProductCenter.WebApp.Config
{
    public static class HealthConfigExtensions
    {
        public static void ConfigHealthCheckers(this IServiceCollection services)
        {
            services.AddTransient<IHealthChecker, ConsulHealthChecker>();
            services.AddTransient<IHealthChecker, DBHealthChecker>();
        }
    }
}

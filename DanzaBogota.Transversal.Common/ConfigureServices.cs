using DanzaBogota.Transversal.Common.Jwt;
using Microsoft.Extensions.DependencyInjection;

namespace DanzaBogota.Transversal.Common
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddTransversalCommonServices(this IServiceCollection services)
        {
            services.AddScoped<IJwtService, JwtService>();

            return services;
        }
    }
}

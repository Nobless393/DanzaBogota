using DanzaBogota.Transversal.Common.Jwt;
using DanzaBogota.Transversal.Common.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace DanzaBogota.Transversal.Common
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddTransversalCommonServices(this IServiceCollection services)
        {
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IFileStorageService, LocalFileStorageService>();
            services.AddHttpContextAccessor();

            return services;
        }
    }
}

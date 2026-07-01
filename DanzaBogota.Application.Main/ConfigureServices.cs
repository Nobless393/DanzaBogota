using DanzaBogota.Application.Interface;
using DanzaBogota.Application.Main.Auth;
using DanzaBogota.Application.Main.Locality;
using Microsoft.Extensions.DependencyInjection;

namespace DanzaBogota.Application.Main
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddApplicationMainServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthApplication, AuthApplication>();
            services.AddScoped<ILocalityApplication, LocalityApplication>();

            return services;
        }
    }
}

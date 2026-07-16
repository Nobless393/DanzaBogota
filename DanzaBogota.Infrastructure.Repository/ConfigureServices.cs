using DanzaBogota.Infrastructure.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace DanzaBogota.Infrastructure.Repository
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddInfrastructureRepositoryServices(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}

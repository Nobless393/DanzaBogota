using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace DanzaBogota.Application.Validator
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddValidationServices(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(typeof(ConfigureServices).Assembly);

            return services;
        }
    }
}

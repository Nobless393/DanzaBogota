using DanzaBogota.Application.Interface;
using DanzaBogota.Application.Interface.EventType;
using DanzaBogota.Application.Main.Auth;
using DanzaBogota.Application.Main.Category;
using DanzaBogota.Application.Main.Event;
using DanzaBogota.Application.Main.EventType;
using DanzaBogota.Application.Main.Group;
using DanzaBogota.Application.Main.Inscription;
using DanzaBogota.Application.Main.Locality;
using DanzaBogota.Application.Main.Participant;
using DanzaBogota.Application.Main.User;
using Microsoft.Extensions.DependencyInjection;

namespace DanzaBogota.Application.Main
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddApplicationMainServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthApplication, AuthApplication>();
            services.AddScoped<ILocalityApplication, LocalityApplication>();
            services.AddScoped<IEventApplication, EventApplication>();
            services.AddScoped<IEventTypeApplication, EventTypeApplication>();
            services.AddScoped<IUserApplication, UserApplication>();
            services.AddScoped<IGroupApplication, GroupApplication>();
            services.AddScoped<IParticipantApplication, ParticipantApplication>();
            services.AddScoped<ICategoryApplication, CategoryApplication>();
            services.AddScoped<IInscriptionApplication, InscriptionApplication>();

            return services;
        }
    }
}

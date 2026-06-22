using DanzaBogota.Domain.Entity.Identities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DanzaBogota.Infrastructure.Data
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddInfrastructureDataServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Registrar el DbContext con SQL Server
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")));

            //// Configurar Identity
            //services.AddIdentity<User, Role>(options =>
            //{
            //    // Reglas de contraseña
            //    options.Password.RequireDigit = true;
            //    options.Password.RequireLowercase = true;
            //    options.Password.RequireUppercase = true;
            //    options.Password.RequireNonAlphanumeric = false;
            //    options.Password.RequiredLength = 8;

            //    // Reglas de usuario
            //    options.User.RequireUniqueEmail = true;

            //    // Bloqueo por intentos fallidos
            //    options.Lockout.MaxFailedAccessAttempts = 5;
            //    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            //})
            //.AddEntityFrameworkStores<ApplicationDbContext>()
            //.AddDefaultTokenProviders();

            return services;
        }
    }
}

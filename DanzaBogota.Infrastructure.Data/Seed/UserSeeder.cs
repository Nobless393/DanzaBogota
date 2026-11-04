using DanzaBogota.Domain.Entity.Identities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace DanzaBogota.Infrastructure.Data.Seed
{
    public static class UserSeeder
    {
        /// <summary>
        /// Crea el usuario administrador inicial a partir de la configuración
        /// (Seed:AdminEmail y Seed:AdminPassword). Las credenciales nunca se guardan
        /// en el código: usa dotnet user-secrets o variables de entorno.
        /// </summary>
        public static async Task SeedAdminUserAsync(UserManager<User> userManager, IConfiguration configuration)
        {
            var adminEmail = configuration["Seed:AdminEmail"];
            var adminPassword = configuration["Seed:AdminPassword"];

            if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
            {
                // Sin credenciales configuradas no se crea el administrador.
                return;
            }

            var existingAdmin = await userManager.FindByEmailAsync(adminEmail);

            if (existingAdmin == null)
            {
                var adminUser = new User
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Admin",
                    LastName = "DanzaBogota",
                    DocumentType = "CC",
                    DocumentNumber = "0000000000",
                    PhoneNumber = "0000000000",
                    IsActive = true,
                    EmailConfirmed = true,
                    Created = DateTimeOffset.UtcNow,
                    CreatedBy = "AdminSeeder",
                    LastModified = DateTimeOffset.UtcNow,
                    LastModifiedBy = "AdminSeeder"
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}

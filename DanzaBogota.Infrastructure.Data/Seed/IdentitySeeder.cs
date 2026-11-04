using DanzaBogota.Domain.Entity.Identities;
using Microsoft.AspNetCore.Identity;

namespace DanzaBogota.Infrastructure.Data.Seed
{
    public static class IdentitySeeder
    {
        public static async Task SeedRolesAsync(RoleManager<Role> roleManager)
        {
            string[] roleNames = { "Admin", "Gestor", "Grupo" };

            foreach (var roleName in roleNames)
            {
                var exists = await roleManager.RoleExistsAsync(roleName);

                if (!exists)
                {
                    await roleManager.CreateAsync(new Role { Name = roleName });
                }
            }
        }
    }
}

using DanzaBogota.Domain.Entity.Identities;
using Microsoft.AspNetCore.Identity;

namespace DanzaBogota.Infrastructure.Data.Seed
{
    public static class UserSeeder
    {
        public static async Task SeedAdminUserAsync(UserManager<User> userManager)
        {
            const string alexanderAdmin = "Alexc312@hotmail.com";

            var existingAdmin = await userManager.FindByEmailAsync(alexanderAdmin);

            if (existingAdmin == null)
            {
                var adminUser = new User
                {
                    UserName = alexanderAdmin,
                    Email = alexanderAdmin,
                    FirstName = "Alexander",
                    LastName = "Castllo",
                    DocumentType = "CC",
                    DocumentNumber = "1234567890",
                    PhoneNumber = "3227869716",
                    IsActive = true,
                    EmailConfirmed = true,
                    Created = DateTimeOffset.UtcNow,
                    CreatedBy = "AdminSeeder",
                    LastModified = DateTimeOffset.UtcNow,
                    LastModifiedBy = "AdminSeeder"
                };

                var result = await userManager.CreateAsync(adminUser, "Admin!123456");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}

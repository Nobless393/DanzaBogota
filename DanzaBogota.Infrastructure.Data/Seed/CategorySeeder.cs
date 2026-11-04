using DanzaBogota.Domain.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace DanzaBogota.Infrastructure.Data.Seed
{
    public static class CategorySeeder
    {
        private static readonly (string Name, int MinAge, int MaxAge, string Description)[] Categories =
        {
            ("Infantil",        4,  11, "Niños entre 4 y 11 años"),
            ("Juvenil",        12,  17, "Jóvenes entre 12 y 17 años"),
            ("Adulto",         18,  59, "Adultos entre 18 y 59 años"),
            ("Adulto Mayor",   60,  99, "Personas de 60 años en adelante"),
            ("Mixta",           4,  99, "Todas las edades")
        };

        public static async Task SeedAsync(ApplicationDbContext context)
        {
            if (!await context.Categories.AnyAsync())
            {
                foreach (var (name, minAge, maxAge, description) in Categories)
                {
                    context.Categories.Add(new Category
                    {
                        Name = name,
                        MinAge = minAge,
                        MaxAge = maxAge,
                        Description = description,
                        IsActive = true,
                        Created = DateTimeOffset.UtcNow,
                        CreatedBy = "CategorySeeder"
                    });
                }

                await context.SaveChangesAsync();
            }
        }
    }
}

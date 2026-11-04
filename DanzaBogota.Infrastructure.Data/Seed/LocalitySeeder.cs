using Microsoft.EntityFrameworkCore;

namespace DanzaBogota.Infrastructure.Data.Seed
{
    public static class LocalitySeeder
    {
        private static readonly string[] Localities =
        {
            "Usaquén", "Chapinero", "Santa Fe", "San Cristóbal", "Usme",
            "Tunjuelito", "Bosa", "Kennedy", "Fontibón", "Engativá",
            "Suba", "Barrios Unidos", "Teusaquillo", "Los Mártires",
            "Antonio Nariño", "Puente Aranda", "La Candelaria",
            "Rafael Uribe Uribe", "Ciudad Bolívar", "Sumapaz"
        };

        public static async Task SeedLocalityAsync(ApplicationDbContext context)
        {
            if (!await context.Localities.AnyAsync())
            {
                foreach (var name in Localities)
                {
                    context.Localities.Add(new Domain.Entity.Entities.Locality
                    {
                        Name = name,
                        Created = DateTimeOffset.UtcNow,
                        CreatedBy = "LocalitySeeder"
                    });
                }

                await context.SaveChangesAsync();
            }
        }
    }
}

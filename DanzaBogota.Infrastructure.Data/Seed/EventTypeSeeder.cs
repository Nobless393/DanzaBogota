using DanzaBogota.Domain.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace DanzaBogota.Infrastructure.Data.Seed
{
    public static class EventTypeSeeder
    {
        private static readonly string[] EventTypes =
        {
            "Festival",
            "Concurso",
            "Taller",
            "Presentación",
            "Clase magistral",
            "Encuentro cultural",
            "Muestra artística"
        };

        public static async Task SeedAsync(ApplicationDbContext context)
        {
            if (!await context.EventTypes.AnyAsync())
            {
                foreach (var name in EventTypes)
                {
                    context.EventTypes.Add(new EventType
                    {
                        Name = name,
                        IsActive = true,
                        Created = DateTimeOffset.UtcNow,
                        CreatedBy = "EventTypeSeeder"
                    });
                }

                await context.SaveChangesAsync();
            }
        }
    }
}

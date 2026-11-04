using DanzaBogota.Domain.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace DanzaBogota.Infrastructure.Data.Configurations
{
    public static class CommonConfiguration
    {
        private const string DefSchema = "COMMON";

        public static void ConfigureCommon(ModelBuilder builder)
        {
            builder.Entity<Locality>(entity =>
            {
                entity.ToTable("Localities", DefSchema);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasIndex(e => e.Name)
                    .IsUnique();

                entity.HasMany(e => e.Communities)
                    .WithOne(e => e.Locality)
                    .HasForeignKey(e => e.LocalityId);

                entity.HasMany(e => e.Events)
                    .WithOne(e => e.Locality)
                    .HasForeignKey(e => e.LocalityId);

                entity.HasMany(e => e.Groups)
                    .WithOne(e => e.Locality)
                    .HasForeignKey(e => e.LocalityId);
            });
        }
    }
}

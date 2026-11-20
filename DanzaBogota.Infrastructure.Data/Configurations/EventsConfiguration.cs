using DanzaBogota.Domain.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace DanzaBogota.Infrastructure.Data.Configurations
{
    public static class EventsConfiguration
    {
        private const string DefSchema = "EVENTS";

        public static void ConfigureEvents(ModelBuilder builder)
        {
            builder.Entity<EventType>(entity =>
            {
                entity.ToTable("EventTypes", DefSchema);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasIndex(e => e.Name)
                    .IsUnique();

                entity.Property(e => e.Description)
                    .HasMaxLength(255);
            });

            builder.Entity<Category>(entity =>
            {
                entity.ToTable("Categories", DefSchema);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasIndex(e => e.Name)
                    .IsUnique();

                entity.Property(e => e.Description)
                    .HasMaxLength(255);

                entity.HasMany(e => e.Groups)
                    .WithOne(e => e.Category)
                    .HasForeignKey(e => e.CategoryId);
            });

            builder.Entity<Event>(entity =>
            {
                entity.ToTable("Events", DefSchema);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Description)
                    .IsRequired();

                entity.Property(e => e.ImageUrl)
                    .HasMaxLength(500);

                entity.Property(e => e.LocationName)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasMaxLength(300);

                entity.Property(e => e.Status)
                    .HasConversion<string>()
                    .HasMaxLength(20);

                entity.HasOne(e => e.Locality)
                    .WithMany(e => e.Events)
                    .HasForeignKey(e => e.LocalityId);

                entity.HasOne(e => e.CreatedByUser)
                    .WithMany(e => e.EventsCreated)
                    .HasForeignKey(e => e.CreatedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(e => e.Resources)
                    .WithOne(e => e.Event)
                    .HasForeignKey(e => e.EventId);

                entity.HasMany(e => e.Applications)
                    .WithOne(e => e.Event)
                    .HasForeignKey(e => e.EventId);

                // Relación N:N implícita
                entity.HasMany(e => e.EventTypes)
                    .WithMany(e => e.Events)
                    .UsingEntity(j => j.ToTable("EventEventTypes", DefSchema));

                entity.HasMany(e => e.Categories)
                    .WithMany(e => e.Events)
                    .UsingEntity(j => j.ToTable("EventCategories", DefSchema));
            });

            builder.Entity<EventResource>(entity =>
            {
                entity.ToTable("EventResources", DefSchema);

                entity.Property(e => e.ResourceType)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Notes)
                    .HasMaxLength(255);

                entity.Property(e => e.FileUrl)
                    .HasMaxLength(500);

                entity.HasOne(e => e.Event)
                    .WithMany(e => e.Resources)
                    .HasForeignKey(e => e.EventId);
            });

            builder.Entity<EventComment>(entity =>
            {
                entity.ToTable("EventComments", DefSchema);

                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(e => e.Event)
                    .WithMany(e => e.Comments)
                    .HasForeignKey(e => e.EventId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Parent)
                    .WithMany(e => e.Replies)
                    .HasForeignKey(e => e.ParentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<EventCommentLike>(entity =>
            {
                entity.ToTable("EventCommentLikes", DefSchema);

                entity.HasIndex(e => new { e.CommentId, e.UserId })
                    .IsUnique();

                entity.HasOne(e => e.Comment)
                    .WithMany(e => e.Likes)
                    .HasForeignKey(e => e.CommentId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}

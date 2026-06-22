using DanzaBogota.Domain.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace DanzaBogota.Infrastructure.Data.Configurations
{
    public static class GroupsConfiguration
    {
        private const string DefSchema = "GROUPS";

        public static void ConfigureGroups(ModelBuilder builder)
        {
            builder.Entity<Group>(entity =>
            {
                entity.ToTable("Groups", DefSchema);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.Description)
                    .HasMaxLength(500);

                entity.Property(e => e.DirectorName)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.Phone)
                    .HasMaxLength(20);

                entity.Property(e => e.Email)
                    .HasMaxLength(150);

                entity.Property(e => e.SocialMediaUrl)
                    .HasMaxLength(300);

                entity.HasOne(e => e.Locality)
                    .WithMany(e => e.Groups)
                    .HasForeignKey(e => e.LocalityId);

                entity.HasOne(e => e.Category)
                    .WithMany(e => e.Groups)
                    .HasForeignKey(e => e.CategoryId);

                entity.HasOne(e => e.User)
                    .WithMany(e => e.Groups)
                    .HasForeignKey(e => e.UserId);

                entity.HasMany(e => e.Participants)
                    .WithOne(e => e.Group)
                    .HasForeignKey(e => e.GroupId);

                entity.HasMany(e => e.Applications)
                    .WithOne(e => e.Group)
                    .HasForeignKey(e => e.GroupId);
            });

            builder.Entity<Participant>(entity =>
            {
                entity.ToTable("Participants", DefSchema);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.DocumentType)
                    .HasConversion<string>()
                    .HasMaxLength(20);

                entity.Property(e => e.DocumentNumber)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.HasOne(e => e.Group)
                    .WithMany(e => e.Participants)
                    .HasForeignKey(e => e.GroupId);

                entity.HasMany(e => e.ApplicationParticipants)
                    .WithOne(e => e.Participant)
                    .HasForeignKey(e => e.ParticipantId);
            });

            builder.Entity<EventApplication>(entity =>
            {
                entity.ToTable("EventApplications", DefSchema);

                entity.Property(e => e.Status)
                    .HasConversion<string>()
                    .HasMaxLength(20);

                entity.Property(e => e.Observations)
                    .HasMaxLength(500);

                entity.HasOne(e => e.Group)
                    .WithMany(e => e.Applications)
                    .HasForeignKey(e => e.GroupId);

                entity.HasOne(e => e.Event)
                    .WithMany(e => e.Applications)
                    .HasForeignKey(e => e.EventId);

                entity.HasMany(e => e.Participants)
                    .WithOne(e => e.Application)
                    .HasForeignKey(e => e.ApplicationId);
            });

            builder.Entity<ApplicationParticipant>(entity =>
            {
                entity.ToTable("ApplicationParticipants", DefSchema);

                entity.HasIndex(e => new { e.ApplicationId, e.ParticipantId })
                    .IsUnique();

                entity.HasOne(e => e.Application)
                    .WithMany(e => e.Participants)
                    .HasForeignKey(e => e.ApplicationId);

                entity.HasOne(e => e.Participant)
                    .WithMany(e => e.ApplicationParticipants)
                    .HasForeignKey(e => e.ParticipantId);
            });
        }
    }
}

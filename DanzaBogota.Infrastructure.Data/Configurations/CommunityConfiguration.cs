using DanzaBogota.Domain.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace DanzaBogota.Infrastructure.Data.Configurations
{
    public static class CommunityConfiguration
    {
        private const string DefSchema = "COMMUNITY";

        public static void ConfigureCommunity(ModelBuilder builder)
        {
            builder.Entity<Community>(entity =>
            {
                entity.ToTable("Communities", DefSchema);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.DanceStyle)
                    .IsRequired()
                    .HasMaxLength(80);

                entity.Property(e => e.Description)
                    .HasMaxLength(500);

                entity.HasOne(e => e.Locality)
                    .WithMany(e => e.Communities)
                    .HasForeignKey(e => e.LocalityId);

                entity.HasMany(e => e.Members)
                    .WithOne(e => e.Community)
                    .HasForeignKey(e => e.CommunityId);

                entity.HasMany(e => e.ChatMessages)
                    .WithOne(e => e.Community)
                    .HasForeignKey(e => e.CommunityId);

                entity.HasMany(e => e.Posts)
                    .WithOne(e => e.Community)
                    .HasForeignKey(e => e.CommunityId);
            });

            builder.Entity<CommunityMember>(entity =>
            {
                entity.ToTable("CommunityMembers", DefSchema);

                entity.HasIndex(e => new { e.CommunityId, e.UserId })
                    .IsUnique();

                entity.HasOne(e => e.Community)
                    .WithMany(e => e.Members)
                    .HasForeignKey(e => e.CommunityId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.User)
                    .WithMany(e => e.CommunityMembers)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<ChatMessage>(entity =>
            {
                entity.ToTable("ChatMessages", DefSchema);

                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.HasOne(e => e.Community)
                    .WithMany(e => e.ChatMessages)
                    .HasForeignKey(e => e.CommunityId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.User)
                    .WithMany(e => e.ChatMessages)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict); 
            });

            builder.Entity<Post>(entity =>
            {
                entity.ToTable("Posts", DefSchema);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Content)
                    .IsRequired();

                entity.HasOne(e => e.Community)
                    .WithMany(e => e.Posts)
                    .HasForeignKey(e => e.CommunityId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.User)
                    .WithMany(e => e.Posts)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict); 

                entity.HasMany(e => e.Comments)
                    .WithOne(e => e.Post)
                    .HasForeignKey(e => e.PostId);
            });

            builder.Entity<PostComment>(entity =>
            {
                entity.ToTable("PostComments", DefSchema);

                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(e => e.Post)
                    .WithMany(e => e.Comments)
                    .HasForeignKey(e => e.PostId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.User)
                    .WithMany(e => e.PostComments)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}

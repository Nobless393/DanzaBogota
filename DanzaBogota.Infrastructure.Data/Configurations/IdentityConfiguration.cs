using DanzaBogota.Domain.Entity.Identities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DanzaBogota.Infrastructure.Data.Configurations
{
    public static class IdentityConfiguration
    {
        private const string DefSchema = "AUTH";

        public static void ConfigureIdentity(ModelBuilder builder)
        {
            builder.Entity<User>(entity =>
            {
                entity.ToTable("Users", DefSchema);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.DocumentType)
                    .HasMaxLength(20);

                entity.Property(e => e.DocumentNumber)
                    .HasMaxLength(20);

                entity.HasIndex(e => e.DocumentNumber)
                    .IsUnique()
                    .HasFilter("[DocumentNumber] IS NOT NULL");

                entity.HasMany(e => e.CommunityMembers)
                    .WithOne(e => e.User)
                    .HasForeignKey(e => e.UserId);

                entity.HasMany(e => e.ChatMessages)
                    .WithOne(e => e.User)
                    .HasForeignKey(e => e.UserId);

                entity.HasMany(e => e.Posts)
                    .WithOne(e => e.User)
                    .HasForeignKey(e => e.UserId);

                entity.HasMany(e => e.PostComments)
                    .WithOne(e => e.User)
                    .HasForeignKey(e => e.UserId);

                entity.HasMany(e => e.Groups)
                    .WithOne(e => e.User)
                    .HasForeignKey(e => e.UserId);

                entity.HasMany(e => e.EventsCreated)
                    .WithOne(e => e.CreatedByUser)
                    .HasForeignKey(e => e.CreatedByUserId);
            });

            builder.Entity<Role>(entity =>
            {
                entity.ToTable("Roles", DefSchema);

                entity.HasMany(e => e.RolePermissions)
                    .WithOne(e => e.Role)
                    .HasForeignKey(e => e.RoleId)
                    .IsRequired();
            });

            builder.Entity<IdentityUserRole<int>>(entity =>
            {
                entity.ToTable("UserRoles", DefSchema);
            });

            builder.Entity<IdentityUserClaim<int>>(entity =>
            {
                entity.ToTable("UserClaims", DefSchema);
            });

            builder.Entity<IdentityUserLogin<int>>(entity =>
            {
                entity.ToTable("UserLogins", DefSchema);
            });

            builder.Entity<IdentityRoleClaim<int>>(entity =>
            {
                entity.ToTable("RoleClaims", DefSchema);
            });

            builder.Entity<IdentityUserToken<int>>(entity =>
            {
                entity.ToTable("UserTokens", DefSchema);
            });

            builder.Entity<Permission>(entity =>
            {
                entity.ToTable("Permissions", DefSchema);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasIndex(e => e.Name)
                    .IsUnique();

                entity.Property(e => e.Description)
                    .HasMaxLength(255);

                entity.HasMany(e => e.RolePermissions)
                    .WithOne(e => e.Permission)
                    .HasForeignKey(e => e.PermissionId)
                    .IsRequired();
            });

            builder.Entity<RolePermission>(entity =>
            {
                entity.ToTable("RolePermissions", DefSchema);

                entity.HasIndex(e => new { e.RoleId, e.PermissionId })
                    .IsUnique();

                entity.HasOne(e => e.Role)
                    .WithMany(e => e.RolePermissions)
                    .HasForeignKey(e => e.RoleId);

                entity.HasOne(e => e.Permission)
                    .WithMany(e => e.RolePermissions)
                    .HasForeignKey(e => e.PermissionId);
            });
        }
    }
}

using DanzaBogota.Domain.Entity.Entities;
using DanzaBogota.Domain.Entity.Identities;
using DanzaBogota.Infrastructure.Data.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DanzaBogota.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }

        public DbSet<Locality> Localities { get; set; }
        public DbSet<Community> Communities { get; set; }
        public DbSet<CommunityMember> CommunityMembers { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostComment> PostComments { get; set; }
        public DbSet<Domain.Entity.Entities.EventType> EventTypes { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventResource> EventResources { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<EventApplication> EventApplications { get; set; }
        public DbSet<ApplicationParticipant> ApplicationParticipants { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            IdentityConfiguration.ConfigureIdentity(modelBuilder);
            CommonConfiguration.ConfigureCommon(modelBuilder);
            CommunityConfiguration.ConfigureCommunity(modelBuilder);
            EventsConfiguration.ConfigureEvents(modelBuilder);
            GroupsConfiguration.ConfigureGroups(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}

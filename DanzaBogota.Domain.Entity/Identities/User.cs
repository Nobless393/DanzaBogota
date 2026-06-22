using DanzaBogota.Domain.Entity.Entities;
using Microsoft.AspNetCore.Identity;

namespace DanzaBogota.Domain.Entity.Identities
{
    /// <summary>
    /// Usuario del sistema, extiende Identity con datos adicionales.
    /// </summary>
    public class User : IdentityUser<int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? DocumentType { get; set; }
        public string? DocumentNumber { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset Created { get; set; }
        public string? CreatedBy { get; set; }
        public DateTimeOffset? LastModified { get; set; }
        public string? LastModifiedBy { get; set; }

        // Relaciones
        public ICollection<CommunityMember> CommunityMembers { get; set; }
        public ICollection<ChatMessage> ChatMessages { get; set; }
        public ICollection<Post> Posts { get; set; }
        public ICollection<PostComment> PostComments { get; set; }
        public ICollection<Group> Groups { get; set; }
        public ICollection<Event> EventsCreated { get; set; }
    }
}

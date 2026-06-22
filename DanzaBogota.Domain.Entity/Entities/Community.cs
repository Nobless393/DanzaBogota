using DanzaBogota.Domain.Entity.Common;

namespace DanzaBogota.Domain.Entity.Entities
{
    public class Community : BaseAuditableEntity<int>
    {
        public string Name { get; set; }
        public string DanceStyle { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }

        // Relaciones
        public int LocalityId { get; set; }
        public Locality Locality { get; set; }

        public ICollection<CommunityMember> Members { get; set; }
        public ICollection<ChatMessage> ChatMessages { get; set; }
        public ICollection<Post> Posts { get; set; }
    }
}

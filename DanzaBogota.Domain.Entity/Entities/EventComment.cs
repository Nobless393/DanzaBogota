using DanzaBogota.Domain.Entity.Common;
using DanzaBogota.Domain.Entity.Identities;

namespace DanzaBogota.Domain.Entity.Entities
{
    public class EventComment : BaseAuditableEntity<int>
    {
        public string Content { get; set; }
        public int? ParentId { get; set; }
        public bool IsDeleted { get; set; }

        // Relaciones
        public int EventId { get; set; }
        public Event Event { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public EventComment? Parent { get; set; }
        public ICollection<EventComment> Replies { get; set; }
        public ICollection<EventCommentLike> Likes { get; set; }
    }
}

using DanzaBogota.Domain.Entity.Common;
using DanzaBogota.Domain.Entity.Enums;
using DanzaBogota.Domain.Entity.Identities;

namespace DanzaBogota.Domain.Entity.Entities
{
    public class Event : BaseAuditableEntity<int>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string LocationName { get; set; }
        public string Address { get; set; }
        public int MaxParticipants { get; set; }
        public int MaxParticipantsPerGroup { get; set; }
        public EventStatus Status { get; set; }
        public bool RequiresRut { get; set; }
        public bool IsActive { get; set; }

        // Relaciones
        public int LocalityId { get; set; }
        public Locality Locality { get; set; }

        public int CreatedByUserId { get; set; }
        public User CreatedByUser { get; set; }

        public ICollection<EventResource> Resources { get; set; }
        public ICollection<EventType> EventTypes { get; set; }
        public ICollection<Category> Categories { get; set; }
        public ICollection<EventApplication> Applications { get; set; }
    }
}

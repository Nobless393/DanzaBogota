using DanzaBogota.Domain.Entity.Common;
using DanzaBogota.Domain.Entity.Enums;

namespace DanzaBogota.Domain.Entity.Entities
{
    public class EventApplication : BaseAuditableEntity<int>
    {
        public DateTimeOffset AppliedAt { get; set; }
        public ApplicationStatus Status { get; set; }
        public string? Observations { get; set; }

        // Relaciones
        public int GroupId { get; set; }
        public Group Group { get; set; }

        public int EventId { get; set; }
        public Event Event { get; set; }

        public ICollection<ApplicationParticipant> Participants { get; set; }
    }
}

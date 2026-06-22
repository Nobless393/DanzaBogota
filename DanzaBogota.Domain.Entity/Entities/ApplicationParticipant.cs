using DanzaBogota.Domain.Entity.Common;

namespace DanzaBogota.Domain.Entity.Entities
{
    public class ApplicationParticipant : BaseEntity<int>
    {
        public DateTimeOffset Created { get; set; }

        // Relaciones
        public int ApplicationId { get; set; }
        public EventApplication Application { get; set; }

        public int ParticipantId { get; set; }
        public Participant Participant { get; set; }
    }
}
using DanzaBogota.Domain.Entity.Common;

namespace DanzaBogota.Domain.Entity.Entities
{
    public class EventResource : BaseEntity<int>
    {
        public string ResourceType { get; set; }
        public int Quantity { get; set; }
        public string? Notes { get; set; }
        public DateTimeOffset Created { get; set; }

        // Relaciones
        public int EventId { get; set; }
        public Event Event { get; set; }
    }
}

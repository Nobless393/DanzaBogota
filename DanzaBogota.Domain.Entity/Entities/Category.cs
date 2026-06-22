using DanzaBogota.Domain.Entity.Common;

namespace DanzaBogota.Domain.Entity.Entities
{
    public class Category : BaseAuditableEntity<int>
    {
        public string Name { get; set; }
        public int MinAge { get; set; }
        public int MaxAge { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }

        // Relaciones
        public ICollection<Event> Events { get; set; }
        public ICollection<Group> Groups { get; set; }
    }
}

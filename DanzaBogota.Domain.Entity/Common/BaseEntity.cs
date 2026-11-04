using DanzaBogota.Domain.Entity.Common.Interfaces;

namespace DanzaBogota.Domain.Entity.Common
{
    public abstract class BaseEntity<TKey> : IEntity<TKey> where TKey : IEquatable<TKey>
    {
        public TKey Id { get; set; }

        private readonly List<DomainEvent> _domainEvents = new();

        public IReadOnlyCollection<DomainEvent> GetDomainEvents()
            => _domainEvents.ToList();

        public void Raise(DomainEvent domainEvent)
            => _domainEvents.Add(domainEvent);

        public void ClearDomainEvents()
            => _domainEvents.Clear();
    }
}

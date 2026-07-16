using DanzaBogota.Application.Dto.Event;
using DanzaBogota.Application.Interface.EventType;
using DanzaBogota.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DanzaBogota.Application.Main.EventType
{
    public class EventTypeApplication : IEventTypeApplication
    {
        private readonly ApplicationDbContext _context;

        public EventTypeApplication(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EventTypeDto>> GetAllAsync()
        {
            return await _context.EventTypes
                .Where(et => et.IsActive)
                .Select(et => new Dto.Event.EventTypeDto
                {
                    Id = et.Id,
                    Name = et.Name,
                    Description = et.Description,
                    IsActive = et.IsActive
                })
                .ToListAsync();
        }

        public async Task<EventTypeDto?> GetByIdAsync(int id)
        {
            var eventType = await _context.EventTypes
                .FirstOrDefaultAsync(et => et.Id == id && et.IsActive);

            if (eventType == null) return null;

            return new Dto.Event.EventTypeDto
            {
                Id = eventType.Id,
                Name = eventType.Name,
                Description = eventType.Description,
                IsActive = eventType.IsActive
            };
        }

        public async Task<int> CreateAsync(EventTypeDto dto)
        {
            var eventType = new Domain.Entity.Entities.EventType
            {
                Name = dto.Name,
                Description = dto.Description,
                IsActive = true,
                Created = DateTimeOffset.UtcNow,
                CreatedBy = "Admin"
            };

            await _context.EventTypes.AddAsync(eventType);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync(EventTypeDto dto)
        {
            var eventType = await _context.EventTypes
                .FirstOrDefaultAsync(et => et.Id == dto.Id && et.IsActive);

            if (eventType == null) return 0;

            eventType.Name = dto.Name;
            eventType.Description = dto.Description;
            eventType.LastModified = DateTimeOffset.UtcNow;

            return await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var eventType = await _context.EventTypes
                .FirstOrDefaultAsync(et => et.Id == id);

            if (eventType == null) return false;

            eventType.IsActive = false;
            eventType.LastModified = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}

using DanzaBogota.Application.Dto.Event;
using DanzaBogota.Application.Interface;
using DanzaBogota.Domain.Entity.Enums;
using DanzaBogota.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DanzaBogota.Application.Main.Event
{
    public class EventApplication : IEventApplication
    {
        private readonly ApplicationDbContext _context;

        public EventApplication(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EventDto>> GetAllAsync()
        {
            return await _context.Events
                .Include(e => e.Locality)
                .Include(e => e.CreatedByUser)
                .Include(e => e.EventTypes)
                .Include(e => e.Categories)
                .Where(e => e.IsActive)
                .Select(e => new EventDto
                {
                    Id = e.Id,
                    Title = e.Title,
                    Description = e.Description,
                    ImageUrl = e.ImageUrl,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    LocationName = e.LocationName,
                    Address = e.Address,
                    LocalityId = e.LocalityId,
                    LocalityName = e.Locality.Name,
                    MaxParticipants = e.MaxParticipants,
                    MaxParticipantsPerGroup = e.MaxParticipantsPerGroup,
                    Status = e.Status.ToString(),
                    IsActive = e.IsActive,
                    CreatedByName = e.CreatedByUser.FirstName + " " + e.CreatedByUser.LastName,
                    EventTypes = e.EventTypes.Select(et => et.Name).ToList(),
                    Categories = e.Categories.Select(c => c.Name).ToList()
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<EventDto>> GetPublishedAsync()
        {
            return await _context.Events
                .Include(e => e.Locality)
                .Include(e => e.CreatedByUser)
                .Include(e => e.EventTypes)
                .Include(e => e.Categories)
                .Where(e => e.IsActive && e.Status == EventStatus.Published)
                .Select(e => new EventDto
                {
                    Id = e.Id,
                    Title = e.Title,
                    Description = e.Description,
                    ImageUrl = e.ImageUrl,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    LocationName = e.LocationName,
                    Address = e.Address,
                    LocalityId = e.LocalityId,
                    LocalityName = e.Locality.Name,
                    MaxParticipants = e.MaxParticipants,
                    MaxParticipantsPerGroup = e.MaxParticipantsPerGroup,
                    Status = e.Status.ToString(),
                    IsActive = e.IsActive,
                    CreatedByName = e.CreatedByUser.FirstName + " " + e.CreatedByUser.LastName,
                    EventTypes = e.EventTypes.Select(et => et.Name).ToList(),
                    Categories = e.Categories.Select(c => c.Name).ToList()
                })
                .ToListAsync();
        }

        public async Task<EventDto?> GetByIdAsync(int id)
        {
            var e = await _context.Events
                .Include(e => e.Locality)
                .Include(e => e.CreatedByUser)
                .Include(e => e.EventTypes)
                .Include(e => e.Categories)
                .FirstOrDefaultAsync(e => e.Id == id && e.IsActive);

            if (e == null) return null;

            return new EventDto
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                ImageUrl = e.ImageUrl,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                LocationName = e.LocationName,
                Address = e.Address,
                LocalityId = e.LocalityId,
                LocalityName = e.Locality.Name,
                MaxParticipants = e.MaxParticipants,
                MaxParticipantsPerGroup = e.MaxParticipantsPerGroup,
                Status = e.Status.ToString(),
                IsActive = e.IsActive,
                CreatedByName = e.CreatedByUser.FirstName + " " + e.CreatedByUser.LastName,
                EventTypes = e.EventTypes.Select(et => et.Name).ToList(),
                Categories = e.Categories.Select(c => c.Name).ToList()
            };
        }

        public async Task<int> CreateAsync(CreateEventDto dto, int createdByUserId)
        {
            var eventTypes = await _context.EventTypes
                .Where(et => dto.EventTypeIds.Contains(et.Id))
                .ToListAsync();

            var categories = await _context.Categories
                .Where(c => dto.CategoryIds.Contains(c.Id))
                .ToListAsync();

            var newEvent = new Domain.Entity.Entities.Event
            {
                Title = dto.Title,
                Description = dto.Description,
                ImageUrl = dto.ImageUrl,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                LocationName = dto.LocationName,
                Address = dto.Address,
                LocalityId = dto.LocalityId,
                MaxParticipants = dto.MaxParticipants,
                MaxParticipantsPerGroup = dto.MaxParticipantsPerGroup,
                Status = EventStatus.Draft,
                IsActive = true,
                CreatedByUserId = createdByUserId,
                Created = DateTimeOffset.UtcNow,
                CreatedBy = createdByUserId.ToString(),
                EventTypes = eventTypes,
                Categories = categories
            };

            await _context.Events.AddAsync(newEvent);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync(UpdateEventDto dto)
        {
            var existingEvent = await _context.Events
                .Include(e => e.EventTypes)
                .Include(e => e.Categories)
                .FirstOrDefaultAsync(e => e.Id == dto.Id && e.IsActive);

            if (existingEvent == null) return 0;

            existingEvent.Title = dto.Title;
            existingEvent.Description = dto.Description;
            existingEvent.ImageUrl = dto.ImageUrl;
            existingEvent.StartDate = dto.StartDate;
            existingEvent.EndDate = dto.EndDate;
            existingEvent.LocationName = dto.LocationName;
            existingEvent.Address = dto.Address;
            existingEvent.LocalityId = dto.LocalityId;
            existingEvent.MaxParticipants = dto.MaxParticipants;
            existingEvent.MaxParticipantsPerGroup = dto.MaxParticipantsPerGroup;
            existingEvent.LastModified = DateTimeOffset.UtcNow;

            var eventTypes = await _context.EventTypes
                .Where(et => dto.EventTypeIds.Contains(et.Id))
                .ToListAsync();

            var categories = await _context.Categories
                .Where(c => dto.CategoryIds.Contains(c.Id))
                .ToListAsync();

            existingEvent.EventTypes.Clear();
            existingEvent.Categories.Clear();
            existingEvent.EventTypes = eventTypes;
            existingEvent.Categories = categories;

            return await _context.SaveChangesAsync();
        }

        public async Task<bool> PublishAsync(int id)
        {
            var existingEvent = await _context.Events
                .FirstOrDefaultAsync(e => e.Id == id && e.IsActive);

            if (existingEvent == null) return false;
            if (existingEvent.Status != EventStatus.Draft) return false;

            existingEvent.Status = EventStatus.Published;
            existingEvent.LastModified = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CancelAsync(int id)
        {
            var existingEvent = await _context.Events
                .FirstOrDefaultAsync(e => e.Id == id && e.IsActive);

            if (existingEvent == null) return false;

            existingEvent.Status = EventStatus.Cancelled;
            existingEvent.LastModified = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existingEvent = await _context.Events
                .FirstOrDefaultAsync(e => e.Id == id);

            if (existingEvent == null) return false;

            existingEvent.IsActive = false;
            existingEvent.LastModified = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}

using DanzaBogota.Application.Dto.Event;
using DanzaBogota.Application.Interface;
using DanzaBogota.Domain.Entity.Enums;
using DanzaBogota.Infrastructure.Data;
using DanzaBogota.Transversal.Common.Responses;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using EventEntity = DanzaBogota.Domain.Entity.Entities.Event;

namespace DanzaBogota.Application.Main.Event
{
    public class EventApplication : IEventApplication
    {
        private readonly ApplicationDbContext _context;

        private static readonly Expression<Func<EventEntity, EventDto>> ToDto = e => new EventDto
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
            Categories = e.Categories.Select(c => c.Name).ToList(),
            EventTypeIds = e.EventTypes.Select(et => et.Id).ToList(),
            CategoryIds = e.Categories.Select(c => c.Id).ToList()
        };

        /// <summary>
        /// Si el evento ya superó su EndDate y no fue cancelado, se muestra como Finished
        /// independientemente del estado guardado en BD.
        /// </summary>
        private static string ResolveStatus(EventEntity e)
        {
            if (e.Status == EventStatus.Cancelled) return "Cancelled";
            if (e.EndDate < DateTimeOffset.UtcNow) return "Finished";
            return e.Status.ToString();
        }

        private static EventDto MapToDto(EventEntity e) => new EventDto
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
            Status = ResolveStatus(e),
            IsActive = e.IsActive,
            CreatedByName = e.CreatedByUser.FirstName + " " + e.CreatedByUser.LastName,
            EventTypes = e.EventTypes.Select(et => et.Name).ToList(),
            Categories = e.Categories.Select(c => c.Name).ToList(),
            EventTypeIds = e.EventTypes.Select(et => et.Id).ToList(),
            CategoryIds = e.Categories.Select(c => c.Id).ToList(),
            Resources = e.Resources?.Select(r => new EventResourceDto
            {
                Id = r.Id,
                ResourceType = r.ResourceType,
                Quantity = r.Quantity,
                Notes = r.Notes,
                FileUrl = r.FileUrl
            }).ToList() ?? new()
        };

        public EventApplication(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedResult<EventDto>> GetAllAsync(EventQueryParameters query)
        {
            var source = _context.Events.Where(e => e.IsActive);
            return await BuildPaginatedAsync(source, query);
        }

        public async Task<PaginatedResult<EventDto>> GetPublishedAsync(EventQueryParameters query)
        {
            var now = DateTimeOffset.UtcNow;
            var source = _context.Events.Where(e =>
                e.IsActive &&
                e.Status == EventStatus.Published &&
                e.EndDate >= now);
            return await BuildPaginatedAsync(source, query);
        }

        public async Task<EventDto?> GetByIdAsync(int id)
        {
            var e = await _context.Events
                .Include(ev => ev.Locality)
                .Include(ev => ev.CreatedByUser)
                .Include(ev => ev.EventTypes)
                .Include(ev => ev.Categories)
                .Include(ev => ev.Resources)
                .Where(ev => ev.Id == id && ev.IsActive)
                .FirstOrDefaultAsync();

            if (e == null) return null;

            return MapToDto(e);
        }

        public async Task<int> CreateAsync(CreateEventDto dto, int createdByUserId)
        {
            var eventTypes = await _context.EventTypes
                .Where(et => dto.EventTypeIds.Contains(et.Id))
                .ToListAsync();

            var categories = await _context.Categories
                .Where(c => dto.CategoryIds.Contains(c.Id))
                .ToListAsync();

            var newEvent = new EventEntity
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

            var now = DateTimeOffset.UtcNow;
            if (existingEvent.EndDate < now)
                throw new InvalidOperationException("No se puede editar un evento que ya ha finalizado.");
            if (existingEvent.StartDate <= now.AddDays(5))
                throw new InvalidOperationException("No se puede editar un evento con menos de 5 días de anticipación al inicio.");

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

            var now = DateTimeOffset.UtcNow;
            if (existingEvent.EndDate < now)
                throw new InvalidOperationException("No se puede cancelar un evento que ya ha finalizado.");
            if (existingEvent.StartDate <= now.AddDays(5))
                throw new InvalidOperationException("No se puede cancelar un evento con menos de 5 días de anticipación al inicio.");

            existingEvent.Status = EventStatus.Cancelled;
            existingEvent.LastModified = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetFinishedCountAsync()
        {
            var now = DateTimeOffset.UtcNow;
            return await _context.Events.CountAsync(e =>
                e.IsActive &&
                e.Status != EventStatus.Cancelled &&
                e.EndDate < now);
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

        private async Task<PaginatedResult<EventDto>> BuildPaginatedAsync(
            IQueryable<EventEntity> source,
            EventQueryParameters query)
        {
            var filtered = ApplyFilters(source, query);

            var totalItems = await filtered.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)query.PageSize);

            var entities = await filtered
                .Include(e => e.Locality)
                .Include(e => e.CreatedByUser)
                .Include(e => e.EventTypes)
                .Include(e => e.Categories)
                .Include(e => e.Resources)
                .OrderByDescending(e => e.StartDate)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            var items = entities.Select(MapToDto).ToList();

            return new PaginatedResult<EventDto>
            {
                Page = query.Page,
                PageSize = query.PageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                Items = items
            };
        }

        private static IQueryable<EventEntity> ApplyFilters(
            IQueryable<EventEntity> source,
            EventQueryParameters query)
        {
            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var term = query.Search.Trim();
                source = source.Where(e =>
                    e.Title.Contains(term) ||
                    e.Description.Contains(term) ||
                    e.LocationName.Contains(term));
            }

            if (query.LocalityId.HasValue)
                source = source.Where(e => e.LocalityId == query.LocalityId.Value);

            if (query.EventTypeId.HasValue)
                source = source.Where(e => e.EventTypes.Any(et => et.Id == query.EventTypeId.Value));

            if (!string.IsNullOrWhiteSpace(query.Status)
                && Enum.TryParse<EventStatus>(query.Status, out var status))
            {
                source = source.Where(e => e.Status == status);
            }

            return source;
        }
    }
}

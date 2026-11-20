using DanzaBogota.Application.Dto.Inscription;
using DanzaBogota.Application.Dto.Participant;
using DanzaBogota.Application.Interface;
using DanzaBogota.Domain.Entity.Enums;
using DanzaBogota.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DanzaBogota.Application.Main.Inscription
{
    public class InscriptionApplication : IInscriptionApplication
    {
        private readonly ApplicationDbContext _context;

        public InscriptionApplication(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(bool Success, string Message)> CreateAsync(CreateInscriptionDto dto, int userId)
        {
            // 1. Verifica que el grupo pertenezca al usuario
            var group = await _context.Groups
                .Include(g => g.Category)
                .Include(g => g.Participants.Where(p => p.IsActive))
                .FirstOrDefaultAsync(g => g.Id == dto.GroupId && g.UserId == userId && g.IsActive);

            if (group == null)
                return (false, "Grupo no encontrado o no tienes permiso.");

            // 2. Verifica que el evento exista y esté publicado
            var eventEntity = await _context.Events
                .Include(e => e.Categories)
                .Include(e => e.Applications)
                .FirstOrDefaultAsync(e => e.Id == dto.EventId && e.IsActive);

            if (eventEntity == null)
                return (false, "Evento no encontrado.");

            if (eventEntity.Status != EventStatus.Published)
                return (false, "El evento no está disponible para inscripciones.");

            // 3. Verifica que la categoría del grupo coincida con las del evento
            var categoryMatch = eventEntity.Categories
                .Any(c => c.Id == group.CategoryId);

            if (!categoryMatch)
                return (false, "La categoría de tu grupo no coincide con las categorías del evento.");

            // 4. Verifica que el grupo no esté ya inscrito
            var alreadyApplied = await _context.EventApplications
                .AnyAsync(a => a.GroupId == dto.GroupId && a.EventId == dto.EventId
                    && a.Status != ApplicationStatus.Cancelled);

            if (alreadyApplied)
                return (false, "Tu grupo ya está inscrito en este evento.");

            // 5. Verifica si el evento requiere RUT
            if (eventEntity.RequiresRut && string.IsNullOrEmpty(group.RutUrl) && !group.UseDirectorRut)
                return (false, "Este evento requiere RUT. Por favor sube el RUT de tu grupo.");

            // 6. Verifica cupos disponibles
            var approvedApplications = eventEntity.Applications
                .Count(a => a.Status == ApplicationStatus.Approved);

            if (approvedApplications >= eventEntity.MaxParticipants)
                return (false, "El evento ha alcanzado el máximo de participantes.");

            // 7. Verifica participantes por grupo
            var selectedParticipantIds = dto.ParticipantIds?.Distinct().ToList() ?? new List<int>();
            var participantCount = selectedParticipantIds.Count > 0
                ? selectedParticipantIds.Count
                : group.Participants.Count;

            if (participantCount > eventEntity.MaxParticipantsPerGroup)
                return (false, $"Tu grupo supera el máximo de {eventEntity.MaxParticipantsPerGroup} participantes por grupo.");

            if (selectedParticipantIds.Count > 0)
            {
                var validParticipantIds = group.Participants.Select(p => p.Id).ToHashSet();
                if (selectedParticipantIds.Any(id => !validParticipantIds.Contains(id)))
                    return (false, "Uno o más participantes seleccionados no pertenecen a tu grupo.");
            }

            // 8. Crea la inscripción
            var inscription = new Domain.Entity.Entities.EventApplication
            {
                GroupId = dto.GroupId,
                EventId = dto.EventId,
                AppliedAt = DateTimeOffset.UtcNow,
                Status = ApplicationStatus.Pending,
                Created = DateTimeOffset.UtcNow,
                CreatedBy = userId.ToString()
            };

            await _context.EventApplications.AddAsync(inscription);
            await _context.SaveChangesAsync();

            // 9. Asocia los participantes del grupo a la inscripción
            var participantsToLink = selectedParticipantIds.Count > 0
                ? group.Participants.Where(p => selectedParticipantIds.Contains(p.Id)).ToList()
                : group.Participants.ToList();

            foreach (var participant in participantsToLink)
            {
                _context.ApplicationParticipants.Add(new Domain.Entity.Entities.ApplicationParticipant
                {
                    ApplicationId = inscription.Id,
                    ParticipantId = participant.Id,
                    Created = DateTimeOffset.UtcNow
                });
            }

            await _context.SaveChangesAsync();

            return (true, "Inscripción realizada correctamente. Pendiente de aprobación.");
        }

        public async Task<IEnumerable<InscriptionDto>> GetMyInscriptionsAsync(int userId)
        {
            return await _context.EventApplications
                .Include(a => a.Group)
                    .ThenInclude(g => g.Category)
                .Include(a => a.Event)
                .Include(a => a.Participants)
                .Where(a => a.Group.UserId == userId)
                .Select(a => new InscriptionDto
                {
                    Id = a.Id,
                    GroupId = a.GroupId,
                    GroupName = a.Group.Name,
                    CategoryName = a.Group.Category.Name,
                    EventId = a.EventId,
                    EventTitle = a.Event.Title,
                    AppliedAt = a.AppliedAt,
                    Status = a.Status.ToString(),
                    Observations = a.Observations,
                    ParticipantCount = a.Participants.Count
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<InscriptionDto>> GetByEventAsync(int eventId)
        {
            return await _context.EventApplications
                .Include(a => a.Group)
                    .ThenInclude(g => g.Category)
                .Include(a => a.Event)
                .Include(a => a.Participants)
                .Where(a => a.EventId == eventId)
                .Select(a => new InscriptionDto
                {
                    Id = a.Id,
                    GroupId = a.GroupId,
                    GroupName = a.Group.Name,
                    CategoryName = a.Group.Category.Name,
                    EventId = a.EventId,
                    EventTitle = a.Event.Title,
                    AppliedAt = a.AppliedAt,
                    Status = a.Status.ToString(),
                    Observations = a.Observations,
                    ParticipantCount = a.Participants.Count
                })
                .ToListAsync();
        }

        public async Task<(bool Success, string Message)> ReviewAsync(ReviewInscriptionDto dto, int userId)
        {
            var inscription = await _context.EventApplications
                .Include(a => a.Event)
                .FirstOrDefaultAsync(a => a.Id == dto.InscriptionId
                    && a.Status == ApplicationStatus.Pending);

            if (inscription == null)
                return (false, "Inscripción no encontrada o ya fue revisada.");

            inscription.Status = dto.Approved
                ? ApplicationStatus.Approved
                : ApplicationStatus.Rejected;

            inscription.Observations = dto.Observations;
            inscription.LastModified = DateTimeOffset.UtcNow;
            inscription.LastModifiedBy = userId.ToString();

            await _context.SaveChangesAsync();

            var statusMsg = dto.Approved ? "aprobada" : "rechazada";
            return (true, $"Inscripción {statusMsg} correctamente.");
        }

        public async Task<(bool Success, string Message)> CancelAsync(int inscriptionId, int userId)
        {
            var inscription = await _context.EventApplications
                .Include(a => a.Group)
                .FirstOrDefaultAsync(a => a.Id == inscriptionId
                    && a.Group.UserId == userId
                    && a.Status == ApplicationStatus.Pending);

            if (inscription == null)
                return (false, "Inscripción no encontrada o no puede cancelarse.");

            inscription.Status = ApplicationStatus.Cancelled;
            inscription.LastModified = DateTimeOffset.UtcNow;
            inscription.LastModifiedBy = userId.ToString();

            await _context.SaveChangesAsync();
            return (true, "Inscripción cancelada correctamente.");
        }

        public async Task<(bool Success, string Message, IEnumerable<ParticipantDto> Data)> GetParticipantsAsync(
            int inscriptionId, int userId, bool isAdminOrGestor)
        {
            var inscription = await _context.EventApplications
                .Include(a => a.Group)
                .Include(a => a.Participants)
                    .ThenInclude(ap => ap.Participant)
                .FirstOrDefaultAsync(a => a.Id == inscriptionId);

            if (inscription == null)
                return (false, "Inscripción no encontrada.", Enumerable.Empty<ParticipantDto>());

            if (!isAdminOrGestor && inscription.Group.UserId != userId)
                return (false, "No tienes permiso para ver esta inscripción.", Enumerable.Empty<ParticipantDto>());

            var data = inscription.Participants
                .Select(ap => new ParticipantDto
                {
                    Id = ap.Participant.Id,
                    FirstName = ap.Participant.FirstName,
                    LastName = ap.Participant.LastName,
                    DocumentType = ap.Participant.DocumentType,
                    DocumentNumber = ap.Participant.DocumentNumber,
                    Age = ap.Participant.Age,
                    BirthDate = ap.Participant.BirthDate,
                    GroupId = ap.Participant.GroupId,
                    GroupName = inscription.Group.Name,
                    IsActive = ap.Participant.IsActive
                });

            return (true, string.Empty, data);
        }

        public async Task<InscriptionStatsDto> GetStatsAsync()
        {
            var stats = await _context.EventApplications
                .GroupBy(_ => _.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            return new InscriptionStatsDto
            {
                PendingCount  = stats.FirstOrDefault(s => s.Status == ApplicationStatus.Pending)?.Count  ?? 0,
                ApprovedCount = stats.FirstOrDefault(s => s.Status == ApplicationStatus.Approved)?.Count ?? 0,
                RejectedCount = stats.FirstOrDefault(s => s.Status == ApplicationStatus.Rejected)?.Count ?? 0,
                TotalCount    = stats.Sum(s => s.Count)
            };
        }

        public async Task<(bool Success, string Message)> UpdateParticipantsAsync(
            int inscriptionId, int userId, bool isAdminOrGestor, UpdateInscriptionParticipantsDto dto)
        {
            var inscription = await _context.EventApplications
                .Include(a => a.Group)
                    .ThenInclude(g => g.Participants.Where(p => p.IsActive))
                .Include(a => a.Event)
                .Include(a => a.Participants)
                .FirstOrDefaultAsync(a => a.Id == inscriptionId);

            if (inscription == null)
                return (false, "Inscripción no encontrada.");

            if (!isAdminOrGestor && inscription.Group.UserId != userId)
                return (false, "No tienes permiso para modificar esta inscripción.");

            if (inscription.Status != ApplicationStatus.Pending)
                return (false, "Solo se pueden modificar los participantes mientras la inscripción esté pendiente.");

            var requestedIds = dto.ParticipantIds.Distinct().ToList();

            if (requestedIds.Count > inscription.Event.MaxParticipantsPerGroup)
                return (false, $"Máximo de {inscription.Event.MaxParticipantsPerGroup} participantes por grupo.");

            var validParticipantIds = inscription.Group.Participants.Select(p => p.Id).ToHashSet();
            if (requestedIds.Any(id => !validParticipantIds.Contains(id)))
                return (false, "Uno o más participantes no pertenecen al grupo.");

            var existing = inscription.Participants.ToList();
            _context.ApplicationParticipants.RemoveRange(existing);

            foreach (var participantId in requestedIds)
            {
                _context.ApplicationParticipants.Add(new Domain.Entity.Entities.ApplicationParticipant
                {
                    ApplicationId = inscription.Id,
                    ParticipantId = participantId,
                    Created = DateTimeOffset.UtcNow
                });
            }

            inscription.LastModified = DateTimeOffset.UtcNow;
            inscription.LastModifiedBy = userId.ToString();

            await _context.SaveChangesAsync();
            return (true, "Participantes actualizados correctamente.");
        }
    }
}

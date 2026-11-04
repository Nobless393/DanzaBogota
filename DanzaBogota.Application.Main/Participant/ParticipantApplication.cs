using DanzaBogota.Application.Dto.Participant;
using DanzaBogota.Application.Interface;
using DanzaBogota.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DanzaBogota.Application.Main.Participant
{
    public class ParticipantApplication : IParticipantApplication
    {
        private readonly ApplicationDbContext _context;

        public ParticipantApplication(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ParticipantDto>> GetByGroupAsync(int groupId, int userId)
        {
            // Verifica que el grupo pertenezca al usuario
            var groupBelongsToUser = await _context.Groups
                .AnyAsync(g => g.Id == groupId && g.UserId == userId && g.IsActive);

            if (!groupBelongsToUser) return Enumerable.Empty<ParticipantDto>();

            return await _context.Participants
                .Include(p => p.Group)
                .Where(p => p.GroupId == groupId && p.IsActive)
                .Select(p => new ParticipantDto
                {
                    Id = p.Id,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    DocumentType = p.DocumentType,
                    DocumentNumber = p.DocumentNumber,
                    Age = p.Age,
                    BirthDate = p.BirthDate,
                    GroupId = p.GroupId,
                    GroupName = p.Group.Name,
                    IsActive = p.IsActive
                })
                .ToListAsync();
        }

        public async Task<ParticipantDto?> GetByIdAsync(int id, int userId)
        {
            var p = await _context.Participants
                .Include(p => p.Group)
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive && p.Group.UserId == userId);

            if (p == null) return null;

            return new ParticipantDto
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                DocumentType = p.DocumentType,
                DocumentNumber = p.DocumentNumber,
                Age = p.Age,
                BirthDate = p.BirthDate,
                GroupId = p.GroupId,
                GroupName = p.Group.Name,
                IsActive = p.IsActive
            };
        }

        public async Task<int> CreateAsync(CreateParticipantDto dto, int userId)
        {
            // Verifica que el grupo pertenezca al usuario
            var groupBelongsToUser = await _context.Groups
                .AnyAsync(g => g.Id == dto.GroupId && g.UserId == userId && g.IsActive);

            if (!groupBelongsToUser) return 0;

            // Calcula la edad automáticamente
            var today = DateOnly.FromDateTime(DateTime.Today);
            var age = today.Year - dto.BirthDate.Year;
            if (dto.BirthDate > today.AddYears(-age)) age--;

            var participant = new Domain.Entity.Entities.Participant
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                DocumentType = dto.DocumentType,
                DocumentNumber = dto.DocumentNumber,
                BirthDate = dto.BirthDate,
                Age = age,
                GroupId = dto.GroupId,
                IsActive = true,
                Created = DateTimeOffset.UtcNow,
                CreatedBy = userId.ToString()
            };

            await _context.Participants.AddAsync(participant);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync(UpdateParticipantDto dto, int userId)
        {
            var participant = await _context.Participants
                .Include(p => p.Group)
                .FirstOrDefaultAsync(p => p.Id == dto.Id
                    && p.IsActive
                    && p.Group.UserId == userId);

            if (participant == null) return 0;

            // Recalcula la edad
            var today = DateOnly.FromDateTime(DateTime.Today);
            var age = today.Year - dto.BirthDate.Year;
            if (dto.BirthDate > today.AddYears(-age)) age--;

            participant.FirstName = dto.FirstName;
            participant.LastName = dto.LastName;
            participant.DocumentType = dto.DocumentType;
            participant.DocumentNumber = dto.DocumentNumber;
            participant.BirthDate = dto.BirthDate;
            participant.Age = age;
            participant.LastModified = DateTimeOffset.UtcNow;
            participant.LastModifiedBy = userId.ToString();

            return await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id, int userId)
        {
            var participant = await _context.Participants
                .Include(p => p.Group)
                .FirstOrDefaultAsync(p => p.Id == id
                    && p.IsActive
                    && p.Group.UserId == userId);

            if (participant == null) return false;

            participant.IsActive = false;
            participant.LastModified = DateTimeOffset.UtcNow;
            participant.LastModifiedBy = userId.ToString();

            await _context.SaveChangesAsync();
            return true;
        }
    }
}

using DanzaBogota.Application.Dto.Group;
using DanzaBogota.Application.Interface;
using DanzaBogota.Infrastructure.Data;
using DanzaBogota.Transversal.Common.Storage;
using Microsoft.EntityFrameworkCore;

namespace DanzaBogota.Application.Main.Group
{
    public class GroupApplication : IGroupApplication
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileStorageService _fileStorageService;

        public GroupApplication(
            ApplicationDbContext context,
            IFileStorageService fileStorageService)
        {
            _context = context;
            _fileStorageService = fileStorageService;
        }

        public async Task<IEnumerable<GroupDto>> GetAllAsync()
        {
            return await _context.Groups
                .Include(g => g.Locality)
                .Include(g => g.Category)
                .Include(g => g.User)
                .Where(g => g.IsActive)
                .Select(g => new GroupDto
                {
                    Id = g.Id,
                    Name = g.Name,
                    Description = g.Description,
                    DirectorName = g.DirectorName,
                    Phone = g.Phone,
                    Email = g.Email,
                    SocialMediaUrl = g.SocialMediaUrl,
                    RutUrl = g.RutUrl,
                    UseDirectorRut = g.UseDirectorRut,
                    LocalityId = g.LocalityId,
                    LocalityName = g.Locality.Name,
                    CategoryId = g.CategoryId,
                    CategoryName = g.Category.Name,
                    IsActive = g.IsActive,
                    DirectorFullName = g.User.FirstName + " " + g.User.LastName
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<GroupDto>> GetMyGroupsAsync(int userId)
        {
            return await _context.Groups
                .Include(g => g.Locality)
                .Include(g => g.Category)
                .Include(g => g.User)
                .Where(g => g.UserId == userId && g.IsActive)
                .Select(g => new GroupDto
                {
                    Id = g.Id,
                    Name = g.Name,
                    Description = g.Description,
                    DirectorName = g.DirectorName,
                    Phone = g.Phone,
                    Email = g.Email,
                    SocialMediaUrl = g.SocialMediaUrl,
                    RutUrl = g.RutUrl,
                    UseDirectorRut = g.UseDirectorRut,
                    LocalityId = g.LocalityId,
                    LocalityName = g.Locality.Name,
                    CategoryId = g.CategoryId,
                    CategoryName = g.Category.Name,
                    IsActive = g.IsActive,
                    DirectorFullName = g.User.FirstName + " " + g.User.LastName
                })
                .ToListAsync();
        }

        public async Task<GroupDto?> GetByIdAsync(int id)
        {
            var g = await _context.Groups
                .Include(g => g.Locality)
                .Include(g => g.Category)
                .Include(g => g.User)
                .FirstOrDefaultAsync(g => g.Id == id && g.IsActive);

            if (g == null) return null;

            return new GroupDto
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
                DirectorName = g.DirectorName,
                Phone = g.Phone,
                Email = g.Email,
                SocialMediaUrl = g.SocialMediaUrl,
                RutUrl = g.RutUrl,
                UseDirectorRut = g.UseDirectorRut,
                LocalityId = g.LocalityId,
                LocalityName = g.Locality.Name,
                CategoryId = g.CategoryId,
                CategoryName = g.Category.Name,
                IsActive = g.IsActive,
                DirectorFullName = g.User.FirstName + " " + g.User.LastName
            };
        }

        public async Task<int> CreateAsync(CreateGroupDto dto, int userId)
        {
            var group = new Domain.Entity.Entities.Group
            {
                Name = dto.Name,
                Description = dto.Description,
                DirectorName = dto.DirectorName,
                Phone = dto.Phone,
                Email = dto.Email,
                SocialMediaUrl = dto.SocialMediaUrl,
                UseDirectorRut = dto.UseDirectorRut,
                LocalityId = dto.LocalityId,
                CategoryId = dto.CategoryId,
                UserId = userId,
                IsActive = true,
                Created = DateTimeOffset.UtcNow,
                CreatedBy = userId.ToString()
            };

            await _context.Groups.AddAsync(group);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync(UpdateGroupDto dto, int userId)
        {
            var group = await _context.Groups
                .FirstOrDefaultAsync(g => g.Id == dto.Id && g.UserId == userId && g.IsActive);

            if (group == null) return 0;

            group.Name = dto.Name;
            group.Description = dto.Description;
            group.DirectorName = dto.DirectorName;
            group.Phone = dto.Phone;
            group.Email = dto.Email;
            group.SocialMediaUrl = dto.SocialMediaUrl;
            group.UseDirectorRut = dto.UseDirectorRut;
            group.LocalityId = dto.LocalityId;
            group.CategoryId = dto.CategoryId;
            group.LastModified = DateTimeOffset.UtcNow;
            group.LastModifiedBy = userId.ToString();

            return await _context.SaveChangesAsync();
        }

        public async Task<bool> UploadRutAsync(int groupId, int userId, Stream fileStream, string fileName)
        {
            var group = await _context.Groups
                .FirstOrDefaultAsync(g => g.Id == groupId && g.UserId == userId && g.IsActive);

            if (group == null) return false;

            // Borra el RUT anterior si existe
            if (!string.IsNullOrEmpty(group.RutUrl))
            {
                await _fileStorageService.DeleteFileAsync(group.RutUrl);
            }

            var rutUrl = await _fileStorageService.SaveFileAsync(fileStream, fileName, "ruts");

            group.RutUrl = rutUrl;
            group.UseDirectorRut = false;
            group.LastModified = DateTimeOffset.UtcNow;
            group.LastModifiedBy = userId.ToString();

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id, int userId)
        {
            var group = await _context.Groups
                .FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);

            if (group == null) return false;

            group.IsActive = false;
            group.LastModified = DateTimeOffset.UtcNow;
            group.LastModifiedBy = userId.ToString();

            await _context.SaveChangesAsync();
            return true;
        }
    }
}

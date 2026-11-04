using DanzaBogota.Application.Dto.Category;
using DanzaBogota.Application.Interface;
using DanzaBogota.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DanzaBogota.Application.Main.Category
{
    public class CategoryApplication : ICategoryApplication
    {
        private readonly ApplicationDbContext _context;

        public CategoryApplication(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllAsync()
        {
            return await _context.Categories
                .Where(c => c.IsActive)
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    MinAge = c.MinAge,
                    MaxAge = c.MaxAge,
                    Description = c.Description
                })
                .ToListAsync();
        }

        public async Task<CategoryDto?> GetByIdAsync(int id)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);

            if (category == null) return null;

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                MinAge = category.MinAge,
                MaxAge = category.MaxAge,
                Description = category.Description
            };
        }
    }
}

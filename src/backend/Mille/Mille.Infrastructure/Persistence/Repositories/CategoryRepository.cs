using Microsoft.EntityFrameworkCore;
using Mille.Domain.Entities;
using Mille.Domain.Interfaces;
using Mille.Infrastructure.Persistence.Data;

namespace Mille.Infrastructure.Persistence.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;
        public CategoryRepository(AppDbContext context) => _context = context;

        public async Task<IEnumerable<Category>> GetAllAsync(CancellationToken ct)
            => await _context.Categories
                .AsNoTracking()
                .ToListAsync(ct);

        public async Task<IEnumerable<Category>> GetAllActiveAsync(CancellationToken ct)
            => await _context.Categories
                .AsNoTracking()
                .Where(c => !c.IsDeleted)
                .ToListAsync(ct);

        public async Task<Category?> GetByIdAsync(int id, CancellationToken ct)
            => await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted, ct);

        public async Task<Category?> GetByNameAsync(string name, CancellationToken ct)
            => await _context.Categories
                .FirstOrDefaultAsync(c => c.Name == name && !c.IsDeleted, ct);

        public async Task<bool> IsExistByName(string name, string? excludeName = null, CancellationToken ct = default)
        {
            return await _context.Categories
                .AnyAsync(c => c.Name == name
                            && !c.IsDeleted
                            && (excludeName == null || c.Name != excludeName), ct);
        }

        public async Task<int> CountAsync(bool includeDeleted = false, CancellationToken ct = default)
        {
            var query = _context.Categories.AsQueryable();

            if (!includeDeleted)
                query = query.Where(c => !c.IsDeleted);

            return await query.CountAsync(ct);
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync(
            string? keyword = null,
            bool includeDeleted = false,
            int page = 1,
            int pageSize = 10,
            CancellationToken ct = default)
        {
            var query = _context.Categories
                .AsNoTracking()
                .AsQueryable();

            if (!includeDeleted)
                query = query.Where(c => !c.IsDeleted);

            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(
                    c => c.Name.Contains(keyword) || 
                    (c.Description != null && c.Description.Contains(keyword)));

            return await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);
        }

        public void Add(Category entity) => _context.Categories.Add(entity);
        public void Update(Category entity) => _context.Categories.Update(entity);
        public void Remove(Category entity) => _context.Categories.Remove(entity);
    }
}

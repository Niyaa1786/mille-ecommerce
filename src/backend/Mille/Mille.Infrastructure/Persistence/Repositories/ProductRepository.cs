using Microsoft.EntityFrameworkCore;
using Mille.Domain.Entities;
using Mille.Domain.Enums;
using Mille.Domain.Interfaces;
using Mille.Infrastructure.Persistence.Data;

namespace Mille.Infrastructure.Persistence.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;
        public ProductRepository(AppDbContext context) => _context = context;

        public async Task<IEnumerable<Product>> GetAllAsync(CancellationToken ct)
            => await _context.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Variants)
                .Include(p => p.Images)
                .Where(p => !p.IsDeleted)
                .ToListAsync(ct);

        public async Task<Product?> GetByIdAsync(Guid id, CancellationToken ct)
            => await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Variants)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted, ct);

        public async Task<Product?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct)
            => await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Variants)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted, ct);

        public async Task<IEnumerable<Product>> GetProductsAsync(
            int? categoryId,
            string? keyword,
            ProductStatus? status,
            bool includeDeleted,
            int page,
            int pageSize,
            CancellationToken ct)
        {
            var query = _context.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Variants)
                .Include(p => p.Images)
                .AsQueryable();

            if (!includeDeleted)
                query = query.Where(p => !p.IsDeleted);

            if(categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId);

            if (status.HasValue)
                query = query.Where(p => p.Status == status);

            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(
                    p => p.Name.Contains(keyword) || 
                    (p.Description != null && p.Description.Contains(keyword)));

            return await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);
        }

        public async Task<int> CountProductsAsync(
            int? categoryId,
            string? keyword,
            ProductStatus? status,
            bool includeDeleted,
            CancellationToken ct)
        {
            var query = _context.Products.AsQueryable();

            if (!includeDeleted)
                query = query.Where(p => !p.IsDeleted);

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            if (status.HasValue)
                query = query.Where(p => p.Status == status.Value);

            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(p => p.Name.Contains(keyword) || (p.Description != null && p.Description.Contains(keyword)));

            return await query.CountAsync(ct);
        }

        public void Add(Product entity) => _context.Products.Add(entity);
        public void Update(Product entity) => _context.Products.Update(entity);
        public void Remove(Product entity) => _context.Products.Remove(entity);
    }
}


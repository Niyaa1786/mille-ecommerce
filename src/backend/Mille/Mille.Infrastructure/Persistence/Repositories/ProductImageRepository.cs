using Microsoft.EntityFrameworkCore;
using Mille.Domain.Entities;
using Mille.Domain.Interfaces;
using Mille.Infrastructure.Persistence.Data;

namespace Mille.Infrastructure.Persistence.Repositories
{
    public class ProductImageRepository : IProductImageRepository
    {
        private readonly AppDbContext _context;
        public ProductImageRepository(AppDbContext context) => _context = context;

        public async Task<IEnumerable<ProductImage>> GetAllAsync(CancellationToken ct)
            => await _context.ProductImages.AsNoTracking().ToListAsync(ct);

        public async Task<ProductImage?> GetByIdAsync(Guid id, CancellationToken ct)
            => await _context.ProductImages.FirstOrDefaultAsync(i => i.Id == id, ct);

        public async Task<IEnumerable<ProductImage>> GetByProductIdAsync(Guid productId, CancellationToken ct)
            => await _context.ProductImages
                .Where(i => i.ProductId == productId)
                .ToListAsync(ct);

        public async Task<IEnumerable<ProductImage>> GetByPublicIdAsync(string publicId, CancellationToken ct)
            => await _context.ProductImages
                .Where(i => i.PublicId == publicId)
                .ToListAsync(ct);

        public void Add(ProductImage entity) => _context.ProductImages.Add(entity);
        public void Update(ProductImage entity) => _context.ProductImages.Update(entity);
        public void Remove(ProductImage entity) => _context.ProductImages.Remove(entity);
    }
}
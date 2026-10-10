using Microsoft.EntityFrameworkCore;
using Mille.Domain.Entities;
using Mille.Domain.Interfaces;
using Mille.Infrastructure.Persistence.Data;

namespace Mille.Infrastructure.Persistence.Repositories
{
    public class ProductVariantRepository : IProductVariantRepository
    {
        private readonly AppDbContext _context;

        public ProductVariantRepository(AppDbContext context) => _context = context;

        public async Task<IEnumerable<ProductVariant>> GetAllAsync(CancellationToken ct)
            => await _context.ProductVariants.AsNoTracking().ToListAsync(ct);

        public async Task<ProductVariant?> GetByIdAsync(Guid id, CancellationToken ct)
            => await _context.ProductVariants.FirstOrDefaultAsync(v => v.Id == id, ct);

        public async Task<ProductVariant?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct)
            => await _context.ProductVariants
                .Include(v => v.Product)
                .FirstOrDefaultAsync(v => v.Id == id, ct);

        public async Task<ProductVariant?> GetBySKUAsync(string sku, CancellationToken ct)
            => await _context.ProductVariants.AsNoTracking().FirstOrDefaultAsync(v => v.SKU == sku, ct);

        public async Task<IEnumerable<ProductVariant>> GetByProductIdAsync(Guid productId, CancellationToken ct)
            => await _context.ProductVariants
                .Where(v => v.ProductId == productId)
                .ToListAsync(ct);

        public async Task<bool> IsExistBySkuAsync(string sku, CancellationToken ct)
            => await _context.ProductVariants.AnyAsync(v => v.SKU == sku, ct);

        public async Task<bool> HasOrdersAsync(Guid variantId, CancellationToken ct)
            => await _context.OrderItems.AnyAsync(i => i.ProductVariantId == variantId, ct);

        public void Add(ProductVariant entity) => _context.ProductVariants.Add(entity);
        public void Update(ProductVariant entity) => _context.ProductVariants.Update(entity);
        public void Remove(ProductVariant entity) => _context.ProductVariants.Remove(entity);
    }
}
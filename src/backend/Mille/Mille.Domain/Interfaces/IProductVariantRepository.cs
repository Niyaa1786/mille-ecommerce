using Mille.Domain.Entities;

namespace Mille.Domain.Interfaces
{
    public interface IProductVariantRepository : IBaseRepository<ProductVariant, Guid>
    {
        Task<ProductVariant?> GetBySKUAsync(string sku, CancellationToken ct = default);
        Task<IEnumerable<ProductVariant>> GetByProductIdAsync(Guid productId, CancellationToken ct = default);
        Task<bool> IsExistBySkuAsync(string sku, CancellationToken ct = default);
        Task<ProductVariant?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default);
        Task<bool> HasOrdersAsync(Guid variantId, CancellationToken ct = default);
    }
}
using Mille.Domain.Entities;

namespace Mille.Domain.Interfaces
{
    public interface IProductImageRepository : IBaseRepository<ProductImage, Guid>
    {
        Task<IEnumerable<ProductImage>> GetByProductIdAsync(Guid productId, CancellationToken ct = default);
        Task<IEnumerable<ProductImage>> GetByPublicIdAsync(string publicId, CancellationToken ct = default);
    }
}

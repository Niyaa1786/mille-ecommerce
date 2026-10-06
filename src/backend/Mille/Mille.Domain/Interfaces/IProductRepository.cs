using Mille.Domain.Entities;
using Mille.Domain.Enums;

namespace Mille.Domain.Interfaces
{
    public interface IProductRepository : IBaseRepository<Product, Guid>
    {
        Task<Product?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default);
        Task<IEnumerable<Product>> GetProductsAsync(
            int? categoryId = null,
            string? keyword = null,
            ProductStatus? status = null,
            bool includeDeleted = false,
            int page = 1,
            int pageSize = 10,
            CancellationToken ct = default);

        Task<int> CountProductsAsync(
            int? categoryId = null,
            string? keyword = null,
            ProductStatus? status = null,
            bool includeDeleted = false,
            CancellationToken ct = default);
    }
}

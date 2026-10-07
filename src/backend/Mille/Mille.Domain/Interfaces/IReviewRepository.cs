using Mille.Domain.Entities;

namespace Mille.Domain.Interfaces
{
    public interface IReviewRepository : IBaseRepository<Review, int>
    {
        Task<bool> IsExistByOrderItemIdAsync(int orderItemId, CancellationToken ct = default);
        Task<IEnumerable<Review>> GetByProductIdAsync(Guid productId, int page, int pageSize, CancellationToken ct = default);
        Task<int> CountByProductIdAsync(Guid productId, CancellationToken ct = default);
        Task<double> GetAverageRatingByProductIdAsync(Guid productId, CancellationToken ct = default);
    }
}

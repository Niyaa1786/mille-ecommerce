using Mille.Domain.Entities;

namespace Mille.Domain.Interfaces
{
    public interface ICouponRepository : IBaseRepository<Coupon, int>
    {
        Task<Coupon?> GetByCodeAsync(string code, CancellationToken ct = default);
        Task<bool> IsExistByCodeAsync(string code, CancellationToken ct = default);
        Task<bool> IsReferencedByOrdersAsync(int couponId, CancellationToken ct = default);

        Task<IEnumerable<Coupon>> GetAllWithFiltersAsync(
            string? keyword = null,
            bool? isActive = null,
            int page = 1,
            int pageSize = 10,
            CancellationToken ct = default);

        Task<int> CountAsync(string? keyword = null, bool? isActive = null, CancellationToken ct = default);
        Task<CouponUsage?> GetUsageByOrderIdAsync(Guid orderId, CancellationToken ct = default);
        Task<bool> IsUsedByUserAsync(int couponId, Guid userId, CancellationToken ct = default);

        void AddUsage(CouponUsage usage);
        void RemoveUsage(CouponUsage usage);
    }
}
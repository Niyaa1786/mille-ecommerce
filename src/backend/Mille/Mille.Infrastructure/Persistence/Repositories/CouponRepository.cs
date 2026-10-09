using Microsoft.EntityFrameworkCore;
using Mille.Domain.Entities;
using Mille.Domain.Interfaces;
using Mille.Infrastructure.Persistence.Data;

namespace Mille.Infrastructure.Persistence.Repositories
{
    public class CouponRepository : ICouponRepository
    {
        private readonly AppDbContext _context;
        public CouponRepository(AppDbContext context) => _context = context;

        public async Task<IEnumerable<Coupon>> GetAllAsync(CancellationToken ct)
            => await _context.Coupons.AsNoTracking().ToListAsync(ct);

        public async Task<Coupon?> GetByIdAsync(int id, CancellationToken ct)
            => await _context.Coupons.FirstOrDefaultAsync(c => c.Id == id, ct);

        public async Task<Coupon?> GetByCodeAsync(string code, CancellationToken ct)
        {
            var normalized = Coupon.NormalizeCode(code);
            return await _context.Coupons.FirstOrDefaultAsync(c => c.Code == normalized, ct);
        }

        public async Task<bool> IsExistByCodeAsync(string code, CancellationToken ct)
        {
            var normalized = Coupon.NormalizeCode(code);
            return await _context.Coupons.AnyAsync(c => c.Code == normalized, ct);
        }

        public async Task<bool> IsReferencedByOrdersAsync(int couponId, CancellationToken ct)
            => await _context.Orders.AnyAsync(o => o.CouponId == couponId, ct);

        public async Task<IEnumerable<Coupon>> GetAllWithFiltersAsync(
            string? keyword,
            bool? isActive,
            int page,
            int pageSize,
            CancellationToken ct)
        {
            var query = _context.Coupons
                .AsNoTracking()
                .AsQueryable();

            if (isActive.HasValue)
                query = query.Where(c => c.IsActive == isActive.Value);

            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(c =>
                    c.Code.Contains(keyword) ||
                    (c.Description != null && c.Description.Contains(keyword)));

            return await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);
        }

        public async Task<int> CountAsync(string? keyword, bool? isActive, CancellationToken ct)
        {
            var query = _context.Coupons.AsQueryable();

            if (isActive.HasValue)
                query = query.Where(c => c.IsActive == isActive.Value);

            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(c =>
                    c.Code.Contains(keyword) ||
                    (c.Description != null && c.Description.Contains(keyword)));

            return await query.CountAsync(ct);
        }

        public async Task<CouponUsage?> GetUsageByOrderIdAsync(Guid orderId, CancellationToken ct)
            => await _context.CouponUsages.FirstOrDefaultAsync(u => u.OrderId == orderId, ct);

        public async Task<bool> IsUsedByUserAsync(int couponId, Guid userId, CancellationToken ct)
                    => await _context.CouponUsages.AnyAsync(u => u.CouponId == couponId && u.UserId == userId, ct);

        public void AddUsage(CouponUsage usage) => _context.CouponUsages.Add(usage);
        public void RemoveUsage(CouponUsage usage) => _context.CouponUsages.Remove(usage);

        public void Add(Coupon entity) => _context.Coupons.Add(entity);
        public void Update(Coupon entity) => _context.Coupons.Update(entity);
        public void Remove(Coupon entity) => _context.Coupons.Remove(entity);
    }
}

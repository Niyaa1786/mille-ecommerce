using Microsoft.EntityFrameworkCore;
using Mille.Domain.Entities;
using Mille.Domain.Interfaces;
using Mille.Infrastructure.Persistence.Data;

namespace Mille.Infrastructure.Persistence.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly AppDbContext _context;
        public ReviewRepository(AppDbContext context) => _context = context;

        public async Task<IEnumerable<Review>> GetAllAsync(CancellationToken ct)
            => await _context.Reviews.AsNoTracking().ToListAsync(ct);

        public async Task<Review?> GetByIdAsync(int id, CancellationToken ct)
            => await _context.Reviews.FirstOrDefaultAsync(r => r.Id == id, ct);

        public async Task<bool> IsExistByOrderItemIdAsync(int orderItemId, CancellationToken ct)
            => await _context.Reviews.AnyAsync(r => r.OrderItemId == orderItemId, ct);

        public async Task<IEnumerable<Review>> GetByProductIdAsync(Guid productId, int page, int pageSize, CancellationToken ct)
            => await _context.Reviews
                .AsNoTracking()
                .Include(r => r.User)
                .Include(r => r.OrderItem)
                .Where(r => r.OrderItem.ProductVariant.ProductId == productId)
                .OrderByDescending(r => r.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

        public async Task<int> CountByProductIdAsync(Guid productId, CancellationToken ct)
            => await _context.Reviews.CountAsync(r => r.OrderItem.ProductVariant.ProductId == productId, ct);

        public async Task<double> GetAverageRatingByProductIdAsync(Guid productId, CancellationToken ct)
            => await _context.Reviews
                .Where(r => r.OrderItem.ProductVariant.ProductId == productId)
                .AverageAsync(r => (double?)r.Rating, ct) ?? 0;

        public void Add(Review entity) => _context.Reviews.Add(entity);
        public void Update(Review entity) => _context.Reviews.Update(entity);
        public void Remove(Review entity) => _context.Reviews.Remove(entity);
    }
}

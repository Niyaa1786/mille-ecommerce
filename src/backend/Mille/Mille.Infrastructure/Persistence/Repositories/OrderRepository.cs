using Microsoft.EntityFrameworkCore;
using Mille.Domain.Entities;
using Mille.Domain.Enums;
using Mille.Domain.Interfaces;
using Mille.Infrastructure.Persistence.Data;

namespace Mille.Infrastructure.Persistence.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;
        public OrderRepository(AppDbContext context) => _context = context;

        public async Task<IEnumerable<Order>> GetAllAsync(CancellationToken ct)
            => await _context.Orders.AsNoTracking().ToListAsync(ct);

        public async Task<Order?> GetByIdAsync(Guid id, CancellationToken ct)
            => await _context.Orders.FirstOrDefaultAsync(o => o.Id == id, ct);

        public async Task<Order?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct)
            => await _context.Orders
                .Include(o => o.Items)
                .Include(o => o.StatusHistories)
                .Include(o => o.User)
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o => o.Id == id, ct);

        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(Guid userId, OrderStatus? status, string? keyword, int page, int pageSize, CancellationToken ct)
        {
            var query = _context.Orders
                .AsNoTracking()
                .Include(o => o.Payment)
                .Where(o => o.UserId == userId);

            if (status.HasValue)
                query = query.Where(o => o.Status == status.Value);

            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(o =>
                    o.ReceiverName.Contains(keyword) ||
                    o.ReceiverPhone.Contains(keyword) ||
                    o.ShippingAddress.Contains(keyword));

            return await query
                .OrderByDescending(o => o.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync(OrderStatus? status, string? keyword, int page, int pageSize, CancellationToken ct)
        {
            var query = _context.Orders
                .Include(o => o.Payment)
                .AsNoTracking()
                .AsQueryable();

            if (status.HasValue)
                query = query.Where(o => o.Status == status);

            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(o =>
                    o.ReceiverName.Contains(keyword) ||
                    o.ReceiverPhone.Contains(keyword) ||
                    o.ShippingAddress.Contains(keyword));

            return await query
                .OrderByDescending(o => o.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);
        }

        public async Task<int> CountOrdersByUserIdAsync(Guid userId, OrderStatus? status, string? keyword, CancellationToken ct)
        {
            var query = _context.Orders.Where(o => o.UserId == userId).AsQueryable();

            if (status.HasValue)
                query = query.Where(o => o.Status == status.Value);

            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(o =>
                    o.ReceiverName.Contains(keyword) ||
                    o.ReceiverPhone.Contains(keyword) ||
                    o.ShippingAddress.Contains(keyword));

            return await query.CountAsync(ct);
        }


        public async Task<int> CountOrdersAsync(OrderStatus? status, string? keyword, CancellationToken ct)
        {
            var query = _context.Orders.AsQueryable();

            if (status.HasValue)
                query = query.Where(o => o.Status == status.Value);

            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(o =>
                    o.ReceiverName.Contains(keyword) ||
                    o.ReceiverPhone.Contains(keyword) ||
                    o.ShippingAddress.Contains(keyword));

            return await query.CountAsync(ct);
        }

        public void Add(Order entity) => _context.Orders.Add(entity);
        public void Update(Order entity) => _context.Orders.Update(entity);
        public void Remove(Order entity) => _context.Orders.Remove(entity);

    }
}

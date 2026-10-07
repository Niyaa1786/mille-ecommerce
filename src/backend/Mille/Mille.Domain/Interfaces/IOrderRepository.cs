using Mille.Domain.Entities;
using Mille.Domain.Enums;

namespace Mille.Domain.Interfaces
{
    public interface IOrderRepository : IBaseRepository<Order, Guid>
    {
        Task<IEnumerable<Order>> GetAllWithFiltersAsync(OrderStatus? status, string? keyword, int page, int pageSize, CancellationToken ct = default);
        Task<IEnumerable<Order>> GetAllWithFiltersByUserIdAsync(Guid userId, OrderStatus? status, string? keyword, int page, int pageSize, CancellationToken ct = default);
        Task<Order?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default);
        Task<OrderItem?> GetOrderItemByIdAsync(int orderItemId, CancellationToken ct = default);
        Task<int> CountOrdersAsync(OrderStatus? status, string? keyword, CancellationToken ct = default);
        Task<int> CountOrdersByUserIdAsync(Guid userId, OrderStatus? status, string? keyword, CancellationToken ct = default);
    }
}

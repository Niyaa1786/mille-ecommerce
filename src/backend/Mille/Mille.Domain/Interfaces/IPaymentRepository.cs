using Mille.Domain.Entities;

namespace Mille.Domain.Interfaces
{
    public interface IPaymentRepository : IBaseRepository<Payment, Guid>
    {
        Task<Payment?> GetByOrderIdAsync(Guid orderId, CancellationToken ct = default);
    }
}

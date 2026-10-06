using Microsoft.EntityFrameworkCore;
using Mille.Domain.Entities;
using Mille.Domain.Interfaces;
using Mille.Infrastructure.Persistence.Data;

namespace Mille.Infrastructure.Persistence.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly AppDbContext _context;
        public PaymentRepository(AppDbContext context) => _context = context;

        public async Task<IEnumerable<Payment>> GetAllAsync(CancellationToken ct)
            => await _context.Payments.AsNoTracking().ToListAsync(ct);

        public async Task<Payment?> GetByIdAsync(Guid id, CancellationToken ct)
            => await _context.Payments.FirstOrDefaultAsync(p => p.Id == id, ct);

        public async Task<Payment?> GetByOrderIdAsync(Guid orderId, CancellationToken ct)
            => await _context.Payments.FirstOrDefaultAsync(p => p.OrderId == orderId, ct);

        public void Add(Payment entity) => _context.Payments.Add(entity);
        public void Update(Payment entity) => _context.Payments.Update(entity);
        public void Remove(Payment entity) => _context.Payments.Remove(entity);
    }
}

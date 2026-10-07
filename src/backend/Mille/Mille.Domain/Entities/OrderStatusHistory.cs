using Mille.Domain.Enums;
using Mille.Domain.Exceptions;

namespace Mille.Domain.Entities
{
    public class OrderStatusHistory
    {
        public int Id { get; private set; }
        public Guid OrderId { get; private set; }
        public OrderStatus Status { get; private set; }
        public string? Note { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public Order Order { get; private set; }

        private OrderStatusHistory() { }

        public OrderStatusHistory(Guid orderId, OrderStatus status, string? note = null)
        {
            ValidateRules(orderId, status);

            OrderId = orderId;
            Status = status;
            Note = note;
            CreatedAt = DateTime.UtcNow;
        }

        private static void ValidateRules(Guid orderId, OrderStatus status)
        {
            if (orderId == Guid.Empty)
                throw new DomainException("Order id is required.");

            if (!Enum.IsDefined(status))
                throw new DomainException("Order status is invalid.");
        }
    }
}
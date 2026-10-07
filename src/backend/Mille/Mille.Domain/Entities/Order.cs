using Mille.Domain.Enums;
using Mille.Domain.Exceptions;

namespace Mille.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public string ReceiverName { get; private set; }
        public string ReceiverPhone { get; private set; }
        public string ShippingAddress { get; private set; }
        public decimal TotalAmount { get; private set; }
        public decimal DiscountAmount { get; private set; }
        public int? CouponId { get; private set; }
        public OrderStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        public User User { get; private set; }
        public Payment Payment { get; private set; }
        public Coupon? Coupon { get; private set; }

        public decimal FinalAmount => TotalAmount - DiscountAmount;

        private readonly List<OrderItem> _items = new();
        public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

        private readonly List<OrderStatusHistory> _statusHistories = new();
        public IReadOnlyCollection<OrderStatusHistory> StatusHistories => _statusHistories.AsReadOnly();

        private Order() { }

        public Order(Guid userId, string receiverName, string receiverPhone, string shippingAddress, decimal totalAmount, decimal discountAmount = 0)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            ReceiverName = receiverName;
            ReceiverPhone = receiverPhone;
            ShippingAddress = shippingAddress;
            TotalAmount = totalAmount;
            DiscountAmount = discountAmount;
            Status = OrderStatus.Pending;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;

            AddStatusHistory(OrderStatus.Pending, "The order has been placed.");
        }

        public OrderItem AddItem(Guid productVariantId, string productName, string sku, int quantity, decimal unitPrice)
        {
            if (Status != OrderStatus.Pending)
                throw new DomainException("Only items can be added to orders in the Pending state.");

            var item = new OrderItem(Id, productVariantId, productName, sku, quantity, unitPrice);
            _items.Add(item);
            UpdatedAt = DateTime.UtcNow;
            return item;
        }

        public void ApplyCoupon(int couponId, decimal discountAmount)
        {
            if (Status != OrderStatus.Pending)
                throw new DomainException("Coupon can only be applied to orders in the Pending state.");

            if (discountAmount < 0 || discountAmount > TotalAmount)
                throw new DomainException("Discount amount is invalid.");

            CouponId = couponId;
            DiscountAmount = discountAmount;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AttachPayment(Payment payment)
        {
            Payment = payment;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Confirm(string? note = null)
        {
            ChangeStatus(OrderStatus.Confirmed, note ?? "The order has been confirmed.");
        }

        public void Ship(string? note = null)
        {
            if (Status != OrderStatus.Confirmed)
                throw new DomainException("Only confirmed orders can be shipped.");

            ChangeStatus(OrderStatus.Shipping, note ?? "The order is being shipped.");
        }

        public void Complete(string? note = null)
        {
            if (Status != OrderStatus.Shipping)
                throw new DomainException("Only shipping orders can be completed.");

            ChangeStatus(OrderStatus.Completed, note ?? "The order has been completed.");
        }

        public void Cancel(string reason)
        {
            if (Status == OrderStatus.Completed || Status == OrderStatus.Cancelled)
                throw new DomainException("Only pending or confirmed orders can be cancelled.");

            ChangeStatus(OrderStatus.Cancelled, $"The order has been cancelled. Reason: {reason}");
        }

        private void AddStatusHistory(OrderStatus status, string? note = null)
        {
            var newstatusHistory = new OrderStatusHistory(Id, status, note);
            _statusHistories.Add(newstatusHistory);
        }

        private void ChangeStatus(OrderStatus newStatus, string note)
        {
            Status = newStatus;
            UpdatedAt = DateTime.UtcNow;
            _statusHistories.Add(new OrderStatusHistory(Id, newStatus, note));
        }
    }
}

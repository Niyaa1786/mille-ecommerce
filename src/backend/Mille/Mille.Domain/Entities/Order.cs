using Mille.Domain.Enums;
using Mille.Domain.Exceptions;

namespace Mille.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public string ReceiverName { get; private set; } = string.Empty;
        public string ReceiverPhone { get; private set; } = string.Empty;
        public string ShippingAddress { get; private set; } = string.Empty;
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
            ValidateRules(userId, receiverName, receiverPhone, shippingAddress, totalAmount, discountAmount);

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

            AddStatusHistory(OrderStatus.Pending, "Order has been created.");
        }

        public OrderItem AddItem(Guid productVariantId, string productName, string sku, int quantity, decimal unitPrice)
        {
            EnsureStatus(OrderStatus.Pending, "Items can only be added while the order is pending.");

            var item = new OrderItem(Id, productVariantId, productName, sku, quantity, unitPrice);
            _items.Add(item);
            UpdatedAt = DateTime.UtcNow;
            return item;
        }

        public void ApplyCoupon(int couponId, decimal discountAmount)
        {
            EnsureStatus(OrderStatus.Pending, "Coupon can only be applied to orders in the Pending state.");

            if (discountAmount < 0)
                throw new DomainException("Discount amount cannot be negative.");

            if (discountAmount > TotalAmount)
                throw new DomainException("Discount amount cannot exceed the total amount.");


            CouponId = couponId;
            DiscountAmount = discountAmount;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AttachPayment(Payment payment)
        {
            if (payment is null)
                throw new DomainException("Payment is required.");

            if (payment.OrderId != Id)
                throw new DomainException("Payment does not belong to this order.");

            Payment = payment;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Confirm(string? note = null)
        {
            EnsureStatus(OrderStatus.Pending, "Only pending orders can be confirmed.");

            ChangeStatus(OrderStatus.Confirmed, note ?? "Order has been confirmed.");
        }

        public void Ship(string? note = null)
        {
            EnsureStatus(OrderStatus.Confirmed, "Only confirmed orders can be shipped.");

            ChangeStatus(OrderStatus.Shipping, note ?? "Order is being shipped.");
        }

        public void Complete(string? note = null)
        {
            EnsureStatus(OrderStatus.Shipping, "Only shipping orders can be completed.");

            ChangeStatus(OrderStatus.Completed, note ?? "Order delivered successfully.");
        }

        public void Cancel(string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
                throw new DomainException("Cancellation reason is required.");

            if (Status == OrderStatus.Completed || Status == OrderStatus.Cancelled)
                throw new DomainException("Cannot cancel an order that is already completed or cancelled.");

            ChangeStatus(OrderStatus.Cancelled, $"Order cancelled. Reason: {reason}");
        }

        private void AddStatusHistory(OrderStatus status, string? note = null)
            => _statusHistories.Add(new OrderStatusHistory(Id, status, note));

        private void ChangeStatus(OrderStatus newStatus, string note)
        {
            Status = newStatus;
            UpdatedAt = DateTime.UtcNow;
            AddStatusHistory(newStatus, note);
        }

        private void EnsureStatus(OrderStatus expected, string message)
        {
            if (Status != expected)
                throw new DomainException(message);
        }

        private static void ValidateRules(
            Guid userId,
            string receiverName,
            string receiverPhone,
            string shippingAddress,
            decimal totalAmount,
            decimal discountAmount)
        {
            if (userId == Guid.Empty)
                throw new DomainException("User id is required.");

            if (string.IsNullOrWhiteSpace(receiverName))
                throw new DomainException("Receiver name is required.");

            if (string.IsNullOrWhiteSpace(receiverPhone))
                throw new DomainException("Receiver phone is required.");

            if (string.IsNullOrWhiteSpace(shippingAddress))
                throw new DomainException("Shipping address is required.");

            if (totalAmount < 0)
                throw new DomainException("Total amount cannot be negative.");

            if (discountAmount < 0)
                throw new DomainException("Discount amount cannot be negative.");
        }
    }
}
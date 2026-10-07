using Mille.Domain.Enums;
using Mille.Domain.Exceptions;

namespace Mille.Domain.Entities
{
    public class Payment
    {
        public Guid Id { get; private set; }
        public Guid OrderId { get; private set; }
        public PaymentMethod Method { get; private set; }
        public decimal Amount { get; private set; }
        public string? TransactionId { get; private set; }
        public string? GatewayResponse { get; private set; }
        public PaymentStatus Status { get; private set; }
        public DateTime? PaidAt { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public Order Order { get; private set; }

        private Payment() { }

        public Payment(Guid orderId, PaymentMethod paymentMethod, decimal amount)
        {
            ValidateRules(orderId, paymentMethod, amount);

            Id = Guid.NewGuid();
            OrderId = orderId;
            Method = paymentMethod;
            Amount = amount;
            Status = PaymentStatus.Pending;
            CreatedAt = DateTime.UtcNow;
        }

        public void Complete(string? transactionId = null, string? gatewayResponse = null)
        {
            EnsureStatus(PaymentStatus.Pending, "Only pending payments can be completed.");

            Status = PaymentStatus.Success;
            TransactionId = transactionId;
            GatewayResponse = gatewayResponse;
            PaidAt = DateTime.UtcNow;
        }

        public void Fail(string? gatewayResponse = null)
        {
            EnsureStatus(PaymentStatus.Pending, "Only pending payments can be marked as failed.");

            Status = PaymentStatus.Failed;
            GatewayResponse = gatewayResponse;
        }

        public void Refund()
        {
            EnsureStatus(PaymentStatus.Success, "Only successful payments can be refunded.");

            Status = PaymentStatus.Refunded;
        }

        private void EnsureStatus(PaymentStatus expected, string message)
        {
            if (Status != expected)
                throw new DomainException(message);
        }

        private static void ValidateRules(Guid orderId, PaymentMethod method, decimal amount)
        {
            if (orderId == Guid.Empty)
                throw new DomainException("Order id is required.");

            if (!Enum.IsDefined(method))
                throw new DomainException("Payment method is invalid.");

            if (amount <= 0)
                throw new DomainException("Payment amount must be greater than zero.");
        }
    }
}
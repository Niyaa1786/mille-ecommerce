using Mille.Domain.Enums;

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

        public Order Order {  get; private set; }

        private Payment() { }

        public Payment(Guid orderId, PaymentMethod paymentMethod, decimal amount)
        {
            Id = Guid.NewGuid();
            OrderId = orderId;
            Method = paymentMethod;
            Amount = amount;
            Status = PaymentStatus.Pending;
            CreatedAt = DateTime.UtcNow;
        }

        public void Complete(string? transactionId = null, string? gatewayResponse = null)
        {
            Status = PaymentStatus.Success;
            TransactionId = transactionId;
            GatewayResponse = gatewayResponse;
            PaidAt = DateTime.UtcNow;
        }

        public void Fail(string? gatewayResponse = null)
        {
            Status = PaymentStatus.Failed;
            GatewayResponse = gatewayResponse;
        }

        public void Refund()
        {
            Status = PaymentStatus.Refunded;
        }
    }
}

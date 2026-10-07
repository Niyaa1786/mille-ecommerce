using Mille.Domain.Enums;

namespace Mille.Application.Features.Orders.CreateOrder
{
    public class CreateOrderResponse
    {
        public Guid OrderId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalAmount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string Message { get; set; } = "Order created successfully.";
    }
}

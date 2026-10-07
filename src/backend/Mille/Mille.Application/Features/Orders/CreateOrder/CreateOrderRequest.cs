using Mille.Domain.Enums;
using System.Text.Json.Serialization;

namespace Mille.Application.Features.Orders.CreateOrder
{
    public class CreateOrderRequest
    {
        [JsonIgnore]
        public Guid UserId { get; set; }
        public string ReceiverName { get; set; } = string.Empty;
        public string ReceiverPhone { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        public PaymentMethod PaymentMethod { get; set; }
        public string? CouponCode { get; set; }
    }
}

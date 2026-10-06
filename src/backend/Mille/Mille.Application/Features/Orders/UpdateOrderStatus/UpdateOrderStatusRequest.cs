using Mille.Domain.Enums;

namespace Mille.Application.Features.Orders.UpdateOrderStatus
{
    public class UpdateOrderStatusRequest
    {
        public Guid OrderId { get; set; }
        public OrderStatus NewStatus { get; set; }
        public string? Note { get; set; }
    }
}

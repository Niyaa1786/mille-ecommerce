using System.Text.Json.Serialization;

namespace Mille.Application.Features.Orders.CancelOrder
{
    public class CancelOrderRequest
    {
        [JsonIgnore]
        public Guid UserId { get; set; }
        public Guid OrderId { get; set; }
    }
}

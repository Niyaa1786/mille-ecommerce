using System.Text.Json.Serialization;

namespace Mille.Application.Features.Orders.GetOrder
{
    public class GetOrderRequest
    {
        [JsonIgnore]
        public Guid UserId { get; set; }
        public Guid OrderId { get; set; }
    }
}

using Mille.Domain.Enums;
using System.Text.Json.Serialization;

namespace Mille.Application.Features.Orders.GetOrdersByUserId
{
    public class GetOrdersByUserIdRequest
    {
        [JsonIgnore]
        public Guid UserId { get; set; }
        public OrderStatus? Status {  get; set; }
        public string? Keyword { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}

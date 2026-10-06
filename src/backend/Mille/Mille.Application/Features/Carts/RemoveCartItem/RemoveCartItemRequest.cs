using System.Text.Json.Serialization;

namespace Mille.Application.Features.Carts.RemoveCartItem
{
    public class RemoveCartItemRequest
    {
        [JsonIgnore]
        public Guid UserId { get; set; }
        [JsonIgnore]
        public int CartItemId { get; set; }
    }
}

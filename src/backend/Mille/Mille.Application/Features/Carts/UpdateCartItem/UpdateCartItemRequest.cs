using System.Text.Json.Serialization;

namespace Mille.Application.Features.Carts.UpdateCartItem
{
    public class UpdateCartItemRequest
    {
        [JsonIgnore]
        public Guid UserId { get; set; }
        [JsonIgnore]
        public int CartItemId { get; set; }
        public int Quantity { get; set; }
    }
}

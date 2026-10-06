using System.Text.Json.Serialization;

namespace Mille.Application.Features.Carts.AddToCart
{
    public class AddToCartRequest
    {
        [JsonIgnore]
        public Guid UserId { get; set; }
        public Guid ProductVariantId { get; set; }
        public int Quantity { get; set; }
    }
}

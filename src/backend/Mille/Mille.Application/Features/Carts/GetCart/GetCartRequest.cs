using System.Text.Json.Serialization;

namespace Mille.Application.Features.Carts.GetCart
{
    public class GetCartRequest
    {
        [JsonIgnore]
        public Guid UserId { get; set; }
    }
}

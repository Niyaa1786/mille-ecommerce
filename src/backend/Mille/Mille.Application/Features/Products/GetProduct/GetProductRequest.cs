using System.Text.Json.Serialization;

namespace Mille.Application.Features.Products.GetProduct
{
    public class GetProductRequest
    {
        [JsonIgnore]
        public Guid Id { get; set; }
    }
}

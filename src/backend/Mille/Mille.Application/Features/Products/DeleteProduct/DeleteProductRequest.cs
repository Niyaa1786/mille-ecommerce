using System.Text.Json.Serialization;

namespace Mille.Application.Features.Products.DeleteProduct
{
    public class DeleteProductRequest
    {
        [JsonIgnore]
        public Guid Id { get; set; }
    }
}

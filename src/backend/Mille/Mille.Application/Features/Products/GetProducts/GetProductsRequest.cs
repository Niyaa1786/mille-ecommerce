using Mille.Domain.Enums;

namespace Mille.Application.Features.Products.GetProducts
{
    public class GetProductsRequest
    {
        public int? CategoryId { get; set; }
        public string? Keyword { get; set; }
        public ProductStatus? Status { get; set; }
        public bool IncludeDeleted { get; set; } = false;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}

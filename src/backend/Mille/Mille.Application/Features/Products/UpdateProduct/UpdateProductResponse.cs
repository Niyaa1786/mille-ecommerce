namespace Mille.Application.Features.Products.UpdateProduct
{
    public class UpdateProductResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<ProductVariantDto> Variants { get; set; } = new();
    }
    public class ProductVariantDto
    {
        public Guid Id { get; set; }
        public string SKU { get; set; } = string.Empty;
        public string? Size { get; set; }
        public string? Color { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

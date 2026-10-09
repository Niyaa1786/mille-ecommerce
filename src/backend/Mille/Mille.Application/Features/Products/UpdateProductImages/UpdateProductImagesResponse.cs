namespace Mille.Application.Features.Products.UpdateProductImages
{
    public class UpdateProductImagesResponse
    {
        public Guid ProductId { get; set; }
        public List<ProductImageDto> Images { get; set; } = new();
    }

    public class ProductImageDto
    {
        public Guid Id { get; set; }
        public string PublicId { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsThumbnail { get; set; }
    }
}

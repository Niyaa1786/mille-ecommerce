using Mille.Domain.Exceptions;

namespace Mille.Domain.Entities
{
    public class ProductImage
    {
        public Guid Id { get; private set; }
        public Guid ProductId { get; private set; }
        public string PublicId { get; private set; } = string.Empty;
        public string ImageUrl { get; private set; } = string.Empty;
        public bool IsThumbnail { get; private set; }

        public Product? Product { get; private set; }

        private ProductImage() { }

        public ProductImage(Guid productId, string publicId, string imageUrl, bool isThumbnail = false)
        {
            ValidateRules(productId, publicId, imageUrl);

            Id = Guid.NewGuid();
            ProductId = productId;
            ImageUrl = imageUrl;
            IsThumbnail = isThumbnail;
            PublicId = publicId;
        }

        public void SetThumbnail()
        {
            IsThumbnail = true;
        }

        public void ClearThumbnail()
        {
            IsThumbnail = false;
        }

        private static void ValidateRules(Guid productId, string publicId, string imageUrl)
        {
            if (productId == Guid.Empty)
                throw new DomainException("Product id is required.");

            if (string.IsNullOrWhiteSpace(publicId))
                throw new DomainException("Image public id is required.");

            if (string.IsNullOrWhiteSpace(imageUrl))
                throw new DomainException("Image url is required.");
        }
    }
}
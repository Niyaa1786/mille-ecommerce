using Mille.Domain.Enums;
using Mille.Domain.Exceptions;

namespace Mille.Domain.Entities
{
    public class Product
    {
        public Guid Id { get; private set; }
        public int CategoryId { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string? Description { get; private set; }
        public ProductStatus Status { get; private set; }
        public bool IsDeleted { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        public Category? Category { get; private set; }

        private readonly List<ProductVariant> _variants = new();
        public IReadOnlyCollection<ProductVariant> Variants => _variants.AsReadOnly();

        private readonly List<ProductImage> _images = new();
        public IReadOnlyCollection<ProductImage> Images => _images.AsReadOnly();

        private Product() { }

        public Product(string name, int categoryId, string? description = null, ProductStatus status = ProductStatus.Active)
        {
            Id = Guid.NewGuid();
            Name = name;
            CategoryId = categoryId;
            Description = description;
            Status = status;
            IsDeleted = false;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Update(string name, int categoryId, string? description, ProductStatus status)
        {
            Name = name;
            CategoryId = categoryId;
            Description = description;
            Status = status;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SoftDelete()
        {
            IsDeleted = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public ProductVariant AddVariant(string sku, decimal price, int stock, string? size = null, string? color = null)
        {
            if (_variants.Any(v => v.SKU == sku))
                throw new DomainException($"Variant with SKU '{sku}' already exists.");

            var newVariant = new ProductVariant(this.Id, sku, price, stock, size, color);
            _variants.Add(newVariant);
            UpdatedAt = DateTime.UtcNow;

            return newVariant;
        }

        public void RemoveVariant(Guid variantId)
        {
            var variant = _variants.FirstOrDefault(v => v.Id == variantId);

            if (variant is null)
                throw new DomainException("Variant not found.");

            _variants.Remove(variant);
            UpdatedAt = DateTime.UtcNow;
        }

        public ProductImage AddImage(string imageUrl, string publicId, bool isThumbnail = false)
        {
            if (isThumbnail)
            {
                foreach (var img in _images.Where(i => i.IsThumbnail))
                    img.ClearThumbnail();
            }

            var image = new ProductImage(this.Id, publicId, imageUrl, isThumbnail);
            _images.Add(image);
            UpdatedAt = DateTime.UtcNow;

            return image;
        }

        public void RemoveImage(Guid imageId)
        {
            var image = _images.FirstOrDefault(i => i.Id == imageId);

            if (image is null)
                throw new DomainException("Image not found");

            _images.Remove(image);
            UpdatedAt = DateTime.UtcNow;
        }

        public ProductVariant? GetVariant(Guid varientId)
        {
            return _variants.FirstOrDefault(v => v.Id == varientId);
        }

        public decimal GetCheapestPrice()
        {
            if (_variants.Count == 0)
                return 0;
            return _variants.Min(v => v.Price);
        }

        public ProductVariant? GetCheapestVariant()
        {
            return _variants.MinBy(v => v.Price);
        }

        public List<string> GetPublicIds()
        {
            return _images.Select(i => i.PublicId).ToList();
        }
    }
}

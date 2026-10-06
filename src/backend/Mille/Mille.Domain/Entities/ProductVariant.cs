using Mille.Domain.Exceptions;

namespace Mille.Domain.Entities
{
    public class ProductVariant
    {
        public Guid Id { get; private set; }
        public Guid ProductId { get; private set; }
        public string SKU { get; private set; } = string.Empty;
        public string? Size { get; private set; }
        public string? Color { get; private set; }
        public decimal Price { get; private set; }
        public int Stock { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        public Product? Product { get; private set; }

        private ProductVariant() { }

        public ProductVariant(Guid productId, string sku, decimal price, int stock, string? size = null, string? color = null)
        {

            Id = Guid.NewGuid();
            ProductId = productId;
            SKU = sku;
            Price = price;
            Stock = stock;
            Size = size;
            Color = color;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Update(decimal price, int stock, string? size, string? color)
        {
            Price = price;
            Stock = stock;
            Size = size;
            Color = color;
            UpdatedAt = DateTime.UtcNow;
        }

        public void DeductStock(int quantity)
        {
            if (quantity <= 0)
                throw new DomainException("Quantity to deduct must be greater than zero.");

            if (Stock < quantity)
                throw new DomainException($"Not enough stock for variant {SKU}. Current: {Stock}, Required: {quantity}");

            Stock -= quantity;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Restock(int quantity)
        {
            if (quantity <= 0)
                throw new DomainException("Quantity to restock must be greater than zero.");

            Stock += quantity;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}

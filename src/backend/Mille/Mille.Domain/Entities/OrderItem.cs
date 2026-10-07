using Mille.Domain.Exceptions;

namespace Mille.Domain.Entities
{
    public class OrderItem
    {
        public int Id { get; private set; }
        public Guid OrderId { get; private set; }
        public Guid ProductVariantId { get; private set; }
        public string ProductName { get; private set; } = string.Empty;
        public string SKU { get; private set; } = string.Empty;
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }

        public Order Order { get; private set; }
        public ProductVariant ProductVariant { get; private set; }

        private OrderItem() { }

        public OrderItem(Guid orderId, Guid productVariantId, string productName, string sku, int quantity, decimal unitPrice)
        {
            ValidateRules(orderId, productVariantId, productName, sku, quantity, unitPrice);

            OrderId = orderId;
            ProductVariantId = productVariantId;
            ProductName = productName;
            SKU = sku;
            Quantity = quantity;
            UnitPrice = unitPrice;
        }

        private static void ValidateRules(
            Guid orderId,
            Guid productVariantId,
            string productName,
            string sku,
            int quantity,
            decimal unitPrice)
        {
            if (orderId == Guid.Empty)
                throw new DomainException("Order id is required.");

            if (productVariantId == Guid.Empty)
                throw new DomainException("Product variant id is required.");

            if (string.IsNullOrWhiteSpace(productName))
                throw new DomainException("Product name is required.");

            if (string.IsNullOrWhiteSpace(sku))
                throw new DomainException("SKU is required.");

            if (quantity <= 0)
                throw new DomainException("Quantity must be greater than zero.");

            if (unitPrice < 0)
                throw new DomainException("Unit price cannot be negative.");
        }
    }
}
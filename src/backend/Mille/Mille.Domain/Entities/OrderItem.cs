namespace Mille.Domain.Entities
{
    public class OrderItem
    {
        public int Id { get; private set; }
        public Guid OrderId { get; private set; }
        public Guid ProductVariantId { get; private set; }
        public string ProductName { get; private set; }
        public string SKU { get; private set; }
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }

        public Order Order { get; private set; }
        public ProductVariant ProductVariant { get; private set; }

        private OrderItem() { }

        public OrderItem(Guid orderId, Guid productVariantId, string productName, string sku, int quantity, decimal unitPrice)
        {
            OrderId = orderId;
            ProductVariantId = productVariantId;
            ProductName = productName;
            SKU = sku;
            Quantity = quantity;
            UnitPrice = unitPrice;
        }
    }
}

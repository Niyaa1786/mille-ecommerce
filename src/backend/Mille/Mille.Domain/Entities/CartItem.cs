using Mille.Domain.Exceptions;

namespace Mille.Domain.Entities
{
    public class CartItem
    {
        public int Id { get; private set; }
        public Guid CartId { get; private set; }
        public Guid ProductVariantId { get; private set; }
        public int Quantity { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public Cart Cart { get; private set; }
        public ProductVariant ProductVariant { get; private set; }

        private CartItem() { }

        public CartItem(Guid cartId, Guid variantId, int quantity)
        {
            CartId = cartId;
            ProductVariantId = variantId;
            Quantity = quantity;
            CreatedAt = DateTime.UtcNow;
        }

        public void SetQuantity(int newQuantity)
        {
            if(newQuantity <= 0)
                throw new DomainException("Amount must be positive");
            Quantity = newQuantity;
        }

        public void IncreaseQuantity(int amount)
        {
            if (amount <= 0)
                throw new DomainException("Amount must be positive");
            Quantity += amount;
        }

        public void DecreaseQuantity(int amount)
        {
            if (amount <= 0)
                throw new DomainException("Amount must be positive");
            if(Quantity - amount <= 0)
                throw new DomainException("Quantity cannot be reduced below 1.");

            Quantity -= amount;
        }

        public decimal SubTotal => ProductVariant.Price * Quantity;
    }
}

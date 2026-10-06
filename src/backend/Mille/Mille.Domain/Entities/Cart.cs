using Mille.Domain.Exceptions;

namespace Mille.Domain.Entities
{
    public class Cart
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        private readonly List<CartItem> _items = new();
        public IReadOnlyCollection<CartItem> Items => _items.AsReadOnly();

        public User User { get; private set; }

        private Cart() { }

        public Cart(Guid userId)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public CartItem AddItem(ProductVariant variant, int quantity)
        {
            var existingItem = _items.FirstOrDefault(ci => ci.ProductVariantId == variant.Id);
            if(existingItem == null)
            {
                var newItem = new CartItem(this.Id, variant.Id, quantity);
                _items.Add(newItem);

                UpdatedAt = DateTime.UtcNow;
                return newItem;
            }
            else
            {
                existingItem.IncreaseQuantity(quantity);

                UpdatedAt = DateTime.UtcNow;
                return existingItem;
            }
        }

        public void RemoveItem(int cartItemId)
        {
            var item = _items.FirstOrDefault(ci => ci.Id == cartItemId);
            if (item == null)
                throw new DomainException("Cart item not found");

            _items.Remove(item);
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetItemQuantity(int cartItemId, int newQuantity)
        {
            var item = _items.FirstOrDefault(ci => ci.Id ==  cartItemId);
            if(item == null)
                throw new DomainException("Cart item not found");

            item.SetQuantity(newQuantity);
            UpdatedAt = DateTime.UtcNow;
        }

        public decimal TotalPrice => _items.Sum(ci => ci.SubTotal);
    }
}

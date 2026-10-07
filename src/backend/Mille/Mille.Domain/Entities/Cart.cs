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
            ValidateRules(userId);

            Id = Guid.NewGuid();
            UserId = userId;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public CartItem AddItem(ProductVariant variant, int quantity)
        {
            if (variant is null)
                throw new DomainException("Product variant is required.");

            var existingItem = _items.FirstOrDefault(ci => ci.ProductVariantId == variant.Id);
            if (existingItem == null)
            {
                var newItem = new CartItem(Id, variant.Id, quantity);
                _items.Add(newItem);

                UpdatedAt = DateTime.UtcNow;
                return newItem;
            }

            existingItem.IncreaseQuantity(quantity);
            UpdatedAt = DateTime.UtcNow;
            return existingItem;
        }

        public void RemoveItem(int cartItemId)
        {
            var item = GetItemOrThrow(cartItemId);

            _items.Remove(item);
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetItemQuantity(int cartItemId, int newQuantity)
        {
            var item = GetItemOrThrow(cartItemId);

            item.SetQuantity(newQuantity);
            UpdatedAt = DateTime.UtcNow;
        }

        public decimal TotalPrice => _items.Sum(ci => ci.SubTotal);

        private CartItem GetItemOrThrow(int cartItemId)
            => _items.FirstOrDefault(ci => ci.Id == cartItemId)
               ?? throw new DomainException("Cart item not found.");

        private static void ValidateRules(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new DomainException("User id is required.");
        }
    }
}
namespace Mille.Application.Features.Carts.AddToCart
{
    public class AddToCartResponse
    {
        public string Message { get; set; } = "Item added to cart.";
        public string ProductName { get; set; } = string.Empty;
        public int TotalCartItems { get; set; }
    }
}

namespace Mille.Application.Features.Carts.UpdateCartItem
{
    public class UpdateCartItemResponse
    {
        public int CartItemId { get; set; }
        public int Quantity { get; set; }
        public decimal ItemSubTotal { get; set; }
        public decimal CartTotalAmount { get; set; }
    }
}

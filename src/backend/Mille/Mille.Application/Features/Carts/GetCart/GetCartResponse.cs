using System;
using System.Collections.Generic;
using System.Text;

namespace Mille.Application.Features.Carts.GetCart
{
    public class GetCartResponse
    {
        public Guid CartId { get; set; }
        public List<CartItemDto> Items { get; set; } = new();
        public decimal TotalPrice { get; set; }
    }
    public class CartItemDto
    {
        public int Id { get; set; }
        public Guid ProductVariantId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? SKU { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal => Price * Quantity;
        public string? ThumbnailUrl { get; set; }
    }
}

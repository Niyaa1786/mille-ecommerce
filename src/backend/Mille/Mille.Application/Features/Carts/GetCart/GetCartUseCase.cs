using Mille.Application.Common.Interfaces;

namespace Mille.Application.Features.Carts.GetCart
{
    public class GetCartUseCase : IUseCase<GetCartRequest, GetCartResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetCartUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<GetCartResponse> ExecuteAsync(GetCartRequest request, CancellationToken ct = default)
        {
            var cart = await _unitOfWork.Carts.GetByUserIdWithDetailsAsync(request.UserId, ct);
            if (cart == null)
                return new GetCartResponse
                {
                    CartId = Guid.Empty,
                    Items = new List<CartItemDto>(),
                    TotalPrice = 0,
                };

            var items = cart.Items.Select(ci => new CartItemDto
            {
                Id = ci.Id,
                Quantity = ci.Quantity,
                ProductVariantId = ci.ProductVariantId,
                ProductName = ci.ProductVariant.Product.Name ?? "Unknow",
                SKU = ci.ProductVariant.SKU,
                Price = ci.ProductVariant.Price,
                Color = ci.ProductVariant.Color,
                Size = ci.ProductVariant.Size,
                ThumbnailUrl = ci.ProductVariant.Product.Images.FirstOrDefault(i => i.IsThumbnail).ImageUrl
                            ?? ci.ProductVariant.Product.Images.FirstOrDefault().ImageUrl

            }).ToList();

            var total = items.Sum(ci => ci.Subtotal);

            return new GetCartResponse
            {
                CartId = cart.Id,
                Items = items,
                TotalPrice = total
            };
        }
    }
}

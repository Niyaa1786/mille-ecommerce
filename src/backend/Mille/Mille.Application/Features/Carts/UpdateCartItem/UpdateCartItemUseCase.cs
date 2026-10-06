using FluentValidation;
using Mille.Application.Common.Exceptions;
using Mille.Application.Common.Interfaces;
using Mille.Domain.Entities;

namespace Mille.Application.Features.Carts.UpdateCartItem
{
    public class UpdateCartItemUseCase : IUseCase<UpdateCartItemRequest, UpdateCartItemResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<UpdateCartItemRequest> _validator;
        public UpdateCartItemUseCase(IUnitOfWork unitOfWork, IValidator<UpdateCartItemRequest> validator)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
        }

        public async Task<UpdateCartItemResponse> ExecuteAsync(UpdateCartItemRequest request, CancellationToken ct = default)
        {
            _validator.ValidateAndThrow(request);

            var cart = await _unitOfWork.Carts.GetByUserIdWithDetailsAsync(request.UserId, ct);
            if (cart == null)
                throw new NotFoundException("Cart not found.");

            var cartItem = cart.Items.FirstOrDefault(ci => ci.Id == request.CartItemId);
            if (cartItem == null)
                throw new NotFoundException("Cart item not found.");

            var variant = cartItem.ProductVariant;
            if (variant == null)
                throw new NotFoundException(nameof(ProductVariant), cartItem.ProductVariantId);

            if(variant.Stock < request.Quantity)
                throw new AppValidationException(nameof(request.Quantity), $"Not enough stock. Available: {variant.Stock}");

            cart.SetItemQuantity(request.CartItemId, request.Quantity);

            await _unitOfWork.SaveChangesAsync(ct);

            var subTotal = cartItem.SubTotal;
            var cartTotalAmount = cart.TotalPrice;

            return new UpdateCartItemResponse
            {
                CartItemId = request.CartItemId,
                Quantity = request.Quantity,
                ItemSubTotal = subTotal,
                CartTotalAmount = cartTotalAmount,
            };
        }
    }
}

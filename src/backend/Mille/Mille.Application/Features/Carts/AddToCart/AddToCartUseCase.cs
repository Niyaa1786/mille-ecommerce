using FluentValidation;
using Mille.Application.Common.Exceptions;
using Mille.Application.Common.Interfaces;
using Mille.Domain.Entities;
using Mille.Domain.Enums;

namespace Mille.Application.Features.Carts.AddToCart
{
    public class AddToCartUseCase : IUseCase<AddToCartRequest, AddToCartResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<AddToCartRequest> _validator;
        public AddToCartUseCase(IUnitOfWork unitOfWork, IValidator<AddToCartRequest> validator)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
        }

        public async Task<AddToCartResponse> ExecuteAsync(AddToCartRequest request, CancellationToken ct = default)
        {
            _validator.ValidateAndThrow(request);

            var variant = await _unitOfWork.ProductVariants.GetByIdWithDetailsAsync(request.ProductVariantId, ct);

            if (variant == null)
                throw new NotFoundException(nameof(ProductVariant), request.ProductVariantId);

            var product = variant.Product;
            if (product == null || product.IsDeleted)
                throw new NotFoundException("Product is no longer available.");

            if (product.Status != ProductStatus.Active)
                throw new AppValidationException(nameof(request.ProductVariantId), $"Product '{product.Name}' is currently not available for purchase.");

            if (variant.Stock < request.Quantity)
                throw new AppValidationException(nameof(request.Quantity), $"Not enough stock. Available: {variant.Stock}");

            var cart = await _unitOfWork.Carts.GetByUserIdWithDetailsAsync(request.UserId, ct);
            if (cart == null)
            {
                cart = new Cart(request.UserId);
                _unitOfWork.Carts.Add(cart);
            }

            cart.AddItem(variant, request.Quantity);

            await _unitOfWork.SaveChangesAsync(ct);

            var productName = product.Name;
            var totalCartItems = await _unitOfWork.Carts.CountCartItemsByUserIdAsync(request.UserId, ct);

            return new AddToCartResponse
            {
                ProductName = productName,
                TotalCartItems = totalCartItems,
                Message = $"'{productName}' (x{request.Quantity}) added to cart."
            };
        }
    }
}

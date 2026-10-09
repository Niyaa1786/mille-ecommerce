using FluentValidation;
using Mille.Application.Common.Exceptions;
using Mille.Application.Common.Interfaces;
using Mille.Domain.Entities;
using Mille.Domain.Enums;

namespace Mille.Application.Features.Orders.CreateOrder
{
    public class CreateOrderUseCase(IUnitOfWork unitOfWork, IValidator<CreateOrderRequest> validator) : IUseCase<CreateOrderRequest, CreateOrderResponse>
    {
        public async Task<CreateOrderResponse> ExecuteAsync(CreateOrderRequest request, CancellationToken ct = default)
        {
            validator.ValidateAndThrow(request);

            var user = await unitOfWork.Users.GetByIdAsync(request.UserId, ct);
            if (user == null)
                throw new NotFoundException("User not found.");

            var cart = await unitOfWork.Carts.GetByUserIdWithDetailsAsync(request.UserId, ct);
            if (cart == null || cart.Items.Count == 0)
                throw new AppValidationException(nameof(Cart), "Cart is empty.");

            var totalAmount = cart.TotalPrice;

            var order = new Order(request.UserId, request.ReceiverName, request.ReceiverPhone, request.ShippingAddress, totalAmount);

            foreach (var cartItem in cart.Items)
            {
                var variant = cartItem.ProductVariant;
                if (variant == null)
                    throw new NotFoundException("Product variant not found.");

                var product = variant.Product;
                if (product == null || product.IsDeleted)
                    throw new NotFoundException("Product is no longer available.");

                if (product.Status != ProductStatus.Active)
                    throw new AppValidationException(nameof(cartItem.ProductVariantId), $"Product '{product.Name}' is currently not available for purchase.");

                if (variant.Stock < cartItem.Quantity)
                    throw new AppValidationException(nameof(cartItem.Quantity), $"Not enough stock for variant {variant.SKU}. Available: {variant.Stock}");

                variant.DeductStock(cartItem.Quantity);
                order.AddItem(variant.Id, product.Name, variant.SKU, cartItem.Quantity, variant.Price);
            }

            Coupon? coupon = null;
            if (!string.IsNullOrWhiteSpace(request.CouponCode))
            {
                coupon = await unitOfWork.Coupons.GetByCodeAsync(request.CouponCode, ct);
                if (coupon == null)
                    throw new NotFoundException("Coupon code is invalid.");

                var isUsed = await unitOfWork.Coupons.IsUsedByUserAsync(coupon.Id, request.UserId, ct);
                if (isUsed)
                    throw new AppValidationException(nameof(request.CouponCode), "You have already used this coupon.");

                var discountAmount = coupon.CalculateDiscount(totalAmount);
                order.ApplyCoupon(coupon.Id, discountAmount);
                coupon.Use();
            }

            var payment = new Payment(order.Id, request.PaymentMethod, order.FinalAmount);
            order.AttachPayment(payment);

            unitOfWork.Orders.Add(order);
            if (coupon != null)
                unitOfWork.Coupons.AddUsage(new CouponUsage(coupon.Id, request.UserId, order.Id));
            unitOfWork.Carts.Remove(cart);

            await unitOfWork.SaveChangesAsync(ct);

            return new CreateOrderResponse
            {
                OrderId = order.Id,
                TotalAmount = totalAmount,
                DiscountAmount = order.DiscountAmount,
                FinalAmount = order.FinalAmount,
                PaymentMethod = payment.Method,
                Message = "Order created successfully."
            };
        }
    }
}
using FluentValidation;
using Mille.Application.Common.Exceptions;
using Mille.Application.Common.Interfaces;
using Mille.Domain.Entities;

namespace Mille.Application.Features.Orders.CreateOrder
{
    // =========================================================================================
    //                      🪦 RIP 'private readonly' (2025 - 2026)
    // 
    // 🔮 Thầy bói bảo: Chuyển sang cú pháp mới này bug giảm 50%, ngón tay bớt đau khớp!
    // Bàn phím đã gãy 3 phím Tab vì generate ctor rồi, nên từ UseCase này trở đi 
    // xin phép mở bát Primary Constructor cho  nó "chilllll!!" hands🧋
    // =========================================================================================
    public class CreateOrderUseCase(IUnitOfWork unitOfWork, IValidator<CreateOrderRequest> validator) : IUseCase<CreateOrderRequest, CreateOrderResponse>
    {
        public async Task<CreateOrderResponse> ExecuteAsync(CreateOrderRequest request, CancellationToken ct = default)
        {
            validator.ValidateAndThrow(request);

            var user = await unitOfWork.Users.GetByIdAsync(request.UserId, ct);
            if (user == null)
                throw new NotFoundException("User not found.");

            var cart = await unitOfWork.Carts.GetByUserIdWithDetailsAsync(request.UserId, ct);
            if (cart == null || !cart.Items.Any())
                throw new AppValidationException("Cart", "Cart is empty.");

            var totalAmount = cart.TotalPrice;

            var order = new Order(request.UserId, request.ReceiverName, request.ReceiverPhone, request.ShippingAddress, totalAmount);

            foreach (var cartItem in cart.Items)
            {
                var variant = cartItem.ProductVariant;
                if (variant == null)
                    throw new NotFoundException("Product variant not found.");

                if (variant.Stock < cartItem.Quantity)
                    throw new AppValidationException(nameof(cartItem.Quantity), $"Not enough stock for variant {variant.SKU}. Available: {variant.Stock}");

                variant.DeductStock(cartItem.Quantity);
                order.AddItem(variant.Id, variant.Product?.Name!, variant.SKU, cartItem.Quantity, variant.Price);
            }

            var payment = new Payment(order.Id, request.PaymentMethod, totalAmount);
            order.AttachPayment(payment);

            unitOfWork.Orders.Add(order);
            unitOfWork.Carts.Remove(cart);

            await unitOfWork.SaveChangesAsync(ct);

            return new CreateOrderResponse
            {
                OrderId = order.Id,
                TotalAmount = totalAmount,
                PaymentMethod = payment.Method,
                Message = "Order created successfully."
            };

        }
    }
}

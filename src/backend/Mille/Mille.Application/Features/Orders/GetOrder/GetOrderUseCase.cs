using Mille.Application.Common.Exceptions;
using Mille.Application.Common.Interfaces;

namespace Mille.Application.Features.Orders.GetOrder
{
    public class GetOrderUseCase(IUnitOfWork unitOfWork) : IUseCase<GetOrderRequest, GetOrderResponse>
    {
        public async Task<GetOrderResponse> ExecuteAsync(GetOrderRequest request, CancellationToken ct = default)
        {
            var order = await unitOfWork.Orders.GetByIdWithDetailsAsync(request.OrderId, ct);
            if (order == null)
                throw new NotFoundException("Order not found.");

            return new GetOrderResponse
            {
                Id = order.Id,
                ReceiverName = order.ReceiverName,
                ReceiverPhone = order.ReceiverPhone,
                ShippingAddress = order.ShippingAddress,
                TotalAmount = order.TotalAmount,
                DiscountAmount = order.DiscountAmount,
                CouponCode = order.Coupon?.Code,
                Status = order.Status.ToString(),
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt,
                Items = order.Items.Select(i => new OrderItemDto
                {
                    Id = i.Id,
                    ProductVariantId = i.ProductVariantId,
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    SKU = i.SKU,
                    UnitPrice = i.UnitPrice,
                }).ToList(),
                StatusHistories = order.StatusHistories.Select(h => new OrderStatusHistoryDto
                {
                    Id = h.Id,
                    Status = h.Status.ToString(),
                    Note = h.Note,
                    CreatedAt = h.CreatedAt,
                }).ToList(),
                Payment = new PaymentDto
                {
                    Id = order.Payment.Id,
                    TransactionId = order.Payment.TransactionId,
                    GatewayResponse = order.Payment.GatewayResponse,
                    Method = order.Payment.Method.ToString(),
                    Status = order.Payment.Status.ToString(),
                    Amount = order.Payment.Amount,
                    PaidAt = order.Payment.PaidAt,
                    CreatedAt = order.Payment.CreatedAt,
                }
            };
        }
    }
}

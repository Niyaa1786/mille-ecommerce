using FluentValidation;
using Mille.Application.Common.Exceptions;
using Mille.Application.Common.Interfaces;
using Mille.Domain.Enums;

namespace Mille.Application.Features.Orders.UpdateOrderStatus
{
    public class UpdateOrderStatusUseCase(IUnitOfWork unitOfWork, IValidator<UpdateOrderStatusRequest> validator) : IUseCase<UpdateOrderStatusRequest, UpdateOrderStatusResponse>
    {
        public async Task<UpdateOrderStatusResponse> ExecuteAsync(UpdateOrderStatusRequest request, CancellationToken ct = default)
        {
            validator.ValidateAndThrow(request);

            var order = await unitOfWork.Orders.GetByIdWithDetailsAsync(request.OrderId, ct);
            if (order == null)
                throw new NotFoundException("Order not found.");

            switch (request.NewStatus)
            {
                case OrderStatus.Confirmed:
                    order.Confirm(request.Note); break;
                case OrderStatus.Shipping:
                    order.Ship(request.Note); break;
                case OrderStatus.Completed:
                    order.Complete(request.Note); break;
                case OrderStatus.Cancelled:
                    order.Cancel(request.Note ?? "Canceled by Admin"); break;
                default:
                    throw new AppValidationException(nameof(request.NewStatus), "Invalid status transition.");
            }

            if (request.NewStatus == OrderStatus.Cancelled)
            {
                foreach (var item in order.Items)
                {
                    var variant = await unitOfWork.ProductVariants.GetByIdAsync(item.ProductVariantId, ct);
                    variant?.Restock(item.Quantity);
                }

                if (order.CouponId.HasValue)
                {
                    var coupon = await unitOfWork.Coupons.GetByIdAsync(order.CouponId.Value, ct);
                    coupon?.Release();

                    var usage = await unitOfWork.Coupons.GetUsageByOrderIdAsync(order.Id, ct);
                    if (usage != null)
                        unitOfWork.Coupons.RemoveUsage(usage);
                }

                var payment = order.Payment;
                if (payment != null)
                {
                    if (payment.Status == PaymentStatus.Pending)
                        payment.Fail("Order cancelled.");
                    else if (payment.Status == PaymentStatus.Success)
                        payment.Refund();
                }
            }

            if (request.NewStatus == OrderStatus.Completed)
            {
                var payment = order.Payment;
                if (payment != null && payment.Method == PaymentMethod.COD && payment.Status == PaymentStatus.Pending)
                    payment.Complete();
            }

            await unitOfWork.SaveChangesAsync(ct);

            return new UpdateOrderStatusResponse();
        }
    }
}
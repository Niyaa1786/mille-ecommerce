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

            if (order.Status == OrderStatus.Completed || order.Status == OrderStatus.Cancelled)
                throw new AppValidationException(nameof(request.NewStatus), "Cannot change status of completed or cancelled order.");

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
                    if (variant != null)
                        variant.Restock(item.Quantity);
                }
            }


            if (request.NewStatus == OrderStatus.Completed)
            {
                var payment = await unitOfWork.Payments.GetByOrderIdAsync(order.Id, ct);
                if (payment != null && payment.Method == PaymentMethod.COD && payment.Status == PaymentStatus.Pending)
                    payment.Complete();
            }

            await unitOfWork.SaveChangesAsync(ct);

            return new UpdateOrderStatusResponse();
        }
    }
}

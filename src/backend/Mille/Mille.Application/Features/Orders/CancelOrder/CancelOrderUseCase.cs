using Mille.Application.Common.Exceptions;
using Mille.Application.Common.Interfaces;
using Mille.Domain.Enums;

namespace Mille.Application.Features.Orders.CancelOrder
{
    public class CancelOrderUseCase(IUnitOfWork unitOfWork) : IUseCase<CancelOrderRequest, CancelOrderResponse>
    {
        public async Task<CancelOrderResponse> ExecuteAsync(CancelOrderRequest request, CancellationToken ct = default)
        {
            var order = await unitOfWork.Orders.GetByIdWithDetailsAsync(request.OrderId, ct);
            if (order == null || order.UserId != request.UserId)
                throw new NotFoundException("Order not found.");

            if (order.Status != OrderStatus.Pending)
                throw new AppValidationException(nameof(request.OrderId), "Only pending orders can be cancelled.");

            order.Cancel("Cancelled by user");

            foreach(var item in order.Items)
            {
                var variant = await unitOfWork.ProductVariants.GetByIdAsync(item.ProductVariantId, ct);
                variant.Restock(item.Quantity);
            }

            await unitOfWork.SaveChangesAsync(ct);

            return new CancelOrderResponse();
        }
    }
}

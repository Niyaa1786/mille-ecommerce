using Mille.Application.Common.Interfaces;

namespace Mille.Application.Features.Orders.GetOrdersByUserId
{
    public class GetOrdersByUserIdUseCase(IUnitOfWork unitOfWork) : IUseCase<GetOrdersByUserIdRequest, GetOrdersByUserIdResponse>
    {
        public async Task<GetOrdersByUserIdResponse> ExecuteAsync(GetOrdersByUserIdRequest request, CancellationToken ct = default)
        {
            var orders = await unitOfWork.Orders.GetAllWithFiltersByUserIdAsync(
                request.UserId,
                request.Status,
                request.Keyword,
                request.Page,
                request.PageSize,
                ct);

            var totalOrders = await unitOfWork.Orders.CountOrdersByUserIdAsync(
                request.UserId,
                request.Status,
                request.Keyword,
                ct);

            return new GetOrdersByUserIdResponse
            {
                Orders = orders.Select(o => new OrderSummaryDto
                {
                    Id = o.Id,
                    ReceiverName = o.ReceiverName,
                    ReceiverPhone = o.ReceiverPhone,
                    ShippingAddress = o.ShippingAddress,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status.ToString(),
                    PaymentMethod = o.Payment?.Method.ToString() ?? "N/A",
                    PaymentStatus = o.Payment?.Status.ToString() ?? "N/A",
                    CreatedAt = o.CreatedAt
                }).ToList(),
                TotalCount = totalOrders,
                Page = request.Page,
                PageSize = request.PageSize,
            };
        }
    }
}

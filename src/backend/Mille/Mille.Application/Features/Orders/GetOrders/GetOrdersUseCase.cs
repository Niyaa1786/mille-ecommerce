using Mille.Application.Common.Interfaces;

namespace Mille.Application.Features.Orders.GetOrders
{
    public class GetOrdersUseCase(IUnitOfWork unitOfWork) : IUseCase<GetOrdersRequest, GetOrdersResponse>
    {
        public async Task<GetOrdersResponse> ExecuteAsync(GetOrdersRequest request, CancellationToken ct = default)
        {
            var orders = await unitOfWork.Orders.GetOrdersAsync(
                request.Status,
                request.Keyword,
                request.Page,
                request.PageSize,
                ct);

            var totalOrders = await unitOfWork.Orders.CountOrdersAsync(
                request.Status,
                request.Keyword,
                ct);

            return new GetOrdersResponse
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

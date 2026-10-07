using FluentValidation;
using Mille.Application.Common.Interfaces;

namespace Mille.Application.Features.Coupons.GetCoupons
{
    public class GetCouponsUseCase(IUnitOfWork unitOfWork, IValidator<GetCouponsRequest> validator) : IUseCase<GetCouponsRequest, GetCouponsResponse>
    {
        public async Task<GetCouponsResponse> ExecuteAsync(GetCouponsRequest request, CancellationToken ct = default)
        {
            validator.ValidateAndThrow(request);

            var coupons = await unitOfWork.Coupons.GetAllWithFiltersAsync(
                request.Keyword,
                request.IsActive,
                request.Page,
                request.PageSize,
                ct);

            var totalCount = await unitOfWork.Coupons.CountAsync(request.Keyword, request.IsActive, ct);

            return new GetCouponsResponse
            {
                Items = coupons.Select(c => new CouponDto
                {
                    Id = c.Id,
                    Code = c.Code,
                    Description = c.Description,
                    DiscountType = c.DiscountType.ToString(),
                    DiscountValue = c.DiscountValue,
                    MinOrderAmount = c.MinOrderAmount,
                    MaxDiscountAmount = c.MaxDiscountAmount,
                    Quantity = c.Quantity,
                    UsedCount = c.UsedCount,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt
                }).ToList(),
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }
    }
}

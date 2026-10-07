using FluentValidation;
using Mille.Application.Common.Exceptions;
using Mille.Application.Common.Interfaces;
using Mille.Domain.Entities;

namespace Mille.Application.Features.Coupons.CreateCoupon
{
    public class CreateCouponUseCase(IUnitOfWork unitOfWork, IValidator<CreateCouponRequest> validator) : IUseCase<CreateCouponRequest, CreateCouponResponse>
    {
        public async Task<CreateCouponResponse> ExecuteAsync(CreateCouponRequest request, CancellationToken ct = default)
        {
            validator.ValidateAndThrow(request);

            var isExisting = await unitOfWork.Coupons.IsExistByCodeAsync(request.Code, ct);
            if (isExisting)
                throw new AppValidationException(nameof(request.Code), "Coupon code already exists.");

            var coupon = new Coupon(
                request.Code,
                request.Description,
                request.DiscountType,
                request.DiscountValue,
                request.MinOrderAmount,
                request.MaxDiscountAmount,
                request.Quantity,
                request.StartDate,
                request.EndDate);

            unitOfWork.Coupons.Add(coupon);
            await unitOfWork.SaveChangesAsync(ct);

            return new CreateCouponResponse
            {
                Id = coupon.Id,
                Code = coupon.Code,
                Description = coupon.Description,
                DiscountType = coupon.DiscountType.ToString(),
                DiscountValue = coupon.DiscountValue,
                MinOrderAmount = coupon.MinOrderAmount,
                MaxDiscountAmount = coupon.MaxDiscountAmount,
                Quantity = coupon.Quantity,
                UsedCount = coupon.UsedCount,
                StartDate = coupon.StartDate,
                EndDate = coupon.EndDate,
                IsActive = coupon.IsActive,
                CreatedAt = coupon.CreatedAt
            };
        }
    }
}

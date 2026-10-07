using FluentValidation;
using Mille.Application.Common.Exceptions;
using Mille.Application.Common.Interfaces;
using Mille.Domain.Entities;

namespace Mille.Application.Features.Coupons.UpdateCoupon
{
    public class UpdateCouponUseCase(IUnitOfWork unitOfWork, IValidator<UpdateCouponRequest> validator) : IUseCase<UpdateCouponRequest, UpdateCouponResponse>
    {
        public async Task<UpdateCouponResponse> ExecuteAsync(UpdateCouponRequest request, CancellationToken ct = default)
        {
            validator.ValidateAndThrow(request);

            var coupon = await unitOfWork.Coupons.GetByIdAsync(request.Id, ct);
            if (coupon == null)
                throw new NotFoundException(nameof(Coupon), request.Id);

            coupon.Update(
                request.Description,
                request.DiscountType,
                request.DiscountValue,
                request.MinOrderAmount,
                request.MaxDiscountAmount,
                request.Quantity,
                request.StartDate,
                request.EndDate,
                request.IsActive);

            await unitOfWork.SaveChangesAsync(ct);

            return new UpdateCouponResponse
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

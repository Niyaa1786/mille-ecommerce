using Mille.Application.Common.Exceptions;
using Mille.Application.Common.Interfaces;
using Mille.Domain.Entities;

namespace Mille.Application.Features.Coupons.DeleteCoupon
{
    public class DeleteCouponUseCase(IUnitOfWork unitOfWork) : IUseCase<DeleteCouponRequest, DeleteCouponResponse>
    {
        public async Task<DeleteCouponResponse> ExecuteAsync(DeleteCouponRequest request, CancellationToken ct = default)
        {
            var coupon = await unitOfWork.Coupons.GetByIdAsync(request.Id, ct);
            if (coupon == null)
                throw new NotFoundException(nameof(Coupon), request.Id);

            var isReferenced = await unitOfWork.Coupons.IsReferencedByOrdersAsync(request.Id, ct);
            if (isReferenced)
                throw new AppValidationException(nameof(request.Id), "Coupon has been used by orders and cannot be deleted. Deactivate it instead.");

            unitOfWork.Coupons.Remove(coupon);
            await unitOfWork.SaveChangesAsync(ct);

            return new DeleteCouponResponse();
        }
    }
}

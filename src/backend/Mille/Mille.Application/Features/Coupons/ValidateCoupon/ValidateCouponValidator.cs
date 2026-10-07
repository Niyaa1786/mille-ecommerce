using FluentValidation;

namespace Mille.Application.Features.Coupons.ValidateCoupon
{
    public class ValidateCouponValidator : AbstractValidator<ValidateCouponRequest>
    {
        public ValidateCouponValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Coupon code is required.")
                .MaximumLength(50).WithMessage("Coupon code must not exceed 50 characters.");
        }
    }
}

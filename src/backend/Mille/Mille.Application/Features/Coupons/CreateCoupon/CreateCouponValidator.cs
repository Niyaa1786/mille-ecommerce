using FluentValidation;
using Mille.Domain.Enums;

namespace Mille.Application.Features.Coupons.CreateCoupon
{
    public class CreateCouponValidator : AbstractValidator<CreateCouponRequest>
    {
        public CreateCouponValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Coupon code is required.")
                .MaximumLength(50).WithMessage("Coupon code must not exceed 50 characters.")
                .Matches("^[A-Za-z0-9_-]+$").WithMessage("Coupon code can only contain letters, digits, '-' and '_'.");

            RuleFor(x => x.Description)
                .MaximumLength(255).WithMessage("Description must not exceed 255 characters.");

            RuleFor(x => x.DiscountType)
                .IsInEnum().WithMessage("Discount type is not valid.");

            RuleFor(x => x.DiscountValue)
                .GreaterThan(0).WithMessage("Discount value must be greater than zero.");

            RuleFor(x => x.DiscountValue)
                .LessThanOrEqualTo(100).WithMessage("Percentage discount cannot exceed 100.")
                .When(x => x.DiscountType == DiscountType.Percentage);

            RuleFor(x => x.MinOrderAmount)
                .GreaterThanOrEqualTo(0m).WithMessage("Minimum order amount cannot be negative.")
                .When(x => x.MinOrderAmount.HasValue);

            RuleFor(x => x.MaxDiscountAmount)
                .GreaterThan(0m).WithMessage("Maximum discount amount must be greater than zero.")
                .When(x => x.MaxDiscountAmount.HasValue);

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than zero.")
                .When(x => x.Quantity.HasValue);

            RuleFor(x => x.EndDate)
                .Must((x, endDate) => endDate > x.StartDate).WithMessage("End date must be after start date.")
                .When(x => x.StartDate.HasValue && x.EndDate.HasValue);
        }
    }
}

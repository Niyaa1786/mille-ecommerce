using FluentValidation;

namespace Mille.Application.Features.Carts.UpdateCartItem
{
    public class UpdateCartItemValidator : AbstractValidator<UpdateCartItemRequest>
    {
        public UpdateCartItemValidator()
        {
            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than 0");
        }
    }
}

using FluentValidation;

namespace Mille.Application.Features.Orders.UpdateOrderStatus
{
    public class UpdateOrderStatusValidator : AbstractValidator<UpdateOrderStatusRequest>
    {
        public UpdateOrderStatusValidator()
        {
            RuleFor(x => x.OrderId)
                .NotEmpty().WithMessage("OrderId is required");
            RuleFor(x => x.NewStatus)
                .IsInEnum().WithMessage("Status is not valid.");
        }
    }
}

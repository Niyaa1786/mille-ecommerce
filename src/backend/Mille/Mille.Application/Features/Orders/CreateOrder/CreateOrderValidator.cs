using FluentValidation;

namespace Mille.Application.Features.Orders.CreateOrder
{
    public class CreateOrderValidator : AbstractValidator<CreateOrderRequest>
    {
        public CreateOrderValidator()
        {
            RuleFor(x => x.ReceiverName)
                .NotEmpty().WithMessage("Receiver name is required.")
                .MaximumLength(100);

            RuleFor(x => x.ReceiverPhone)
                .NotEmpty().WithMessage("Receiver phone is required.")
                .MaximumLength(20);

            RuleFor(x => x.ShippingAddress)
                .NotEmpty().WithMessage("Shipping address is required.")
                .MaximumLength(500);
        }
    }
}

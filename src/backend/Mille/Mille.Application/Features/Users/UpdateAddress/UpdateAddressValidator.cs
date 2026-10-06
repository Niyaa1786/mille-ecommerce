using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mille.Application.Features.Users.UpdateAddress
{
    public class UpdateAddressValidator : AbstractValidator<UpdateAddressRequest>
    {
        public UpdateAddressValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Address ID must be valid.");

            RuleFor(x => x.ReceiverName)
                .NotEmpty().WithMessage("Receiver name is required.")
                .MaximumLength(100);

            RuleFor(x => x.ReceiverPhone)
                .NotEmpty().WithMessage("Receiver phone is required.")
                .MaximumLength(20);

            RuleFor(x => x.AddressLine)
                .NotEmpty().WithMessage("Address line is required.")
                .MaximumLength(500);
        }
    }
}

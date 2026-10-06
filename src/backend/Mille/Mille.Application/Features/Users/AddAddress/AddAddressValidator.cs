using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mille.Application.Features.Users.AddAddress
{
    public class AddAddressValidator : AbstractValidator<AddAddressRequest>
    {
        public AddAddressValidator()
        {
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

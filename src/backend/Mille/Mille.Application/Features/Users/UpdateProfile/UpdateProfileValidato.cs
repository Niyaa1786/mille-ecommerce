using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mille.Application.Features.Users.UpdateProfile
{
    public class UpdateProfileValidator : AbstractValidator<UpdateProfileRequest>
    {
        public UpdateProfileValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required.")
                .MaximumLength(100).WithMessage("Full name must not exceed 100 characters.");

            RuleFor(x => x.Phone)
                .MaximumLength(20).WithMessage("Phone must not exceed 20 characters.")
                .When(x => !string.IsNullOrEmpty(x.Phone));
        }
    }
}

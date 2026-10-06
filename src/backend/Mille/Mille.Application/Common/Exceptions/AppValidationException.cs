using FluentValidation.Results;

namespace Mille.Application.Common.Exceptions
{
    public class AppValidationException : FluentValidation.ValidationException
    {
        public AppValidationException(string propertyName, string errorMessage) : base(new[] { new ValidationFailure(propertyName, errorMessage) }) { }
    }
}

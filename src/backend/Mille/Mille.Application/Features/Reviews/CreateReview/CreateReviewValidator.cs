using FluentValidation;
using Mille.Domain.Entities;

namespace Mille.Application.Features.Reviews.CreateReview
{
    public class CreateReviewValidator : AbstractValidator<CreateReviewRequest>
    {
        public CreateReviewValidator()
        {
            RuleFor(x => x.OrderItemId)
                .GreaterThan(0).WithMessage("Order item ID must be valid.");

            RuleFor(x => x.Rating)
                .InclusiveBetween(Review.MinRating, Review.MaxRating)
                .WithMessage($"Rating must be between {Review.MinRating} and {Review.MaxRating}.");

            RuleFor(x => x.Comment)
                .MaximumLength(1000).WithMessage("Comment must not exceed 1000 characters.");
        }
    }
}

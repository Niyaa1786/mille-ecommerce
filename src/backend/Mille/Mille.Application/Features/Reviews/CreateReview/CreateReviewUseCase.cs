using FluentValidation;
using Mille.Application.Common.Exceptions;
using Mille.Application.Common.Interfaces;
using Mille.Domain.Entities;
using Mille.Domain.Enums;

namespace Mille.Application.Features.Reviews.CreateReview
{
    public class CreateReviewUseCase(IUnitOfWork unitOfWork, IValidator<CreateReviewRequest> validator) : IUseCase<CreateReviewRequest, CreateReviewResponse>
    {
        public async Task<CreateReviewResponse> ExecuteAsync(CreateReviewRequest request, CancellationToken ct = default)
        {
            validator.ValidateAndThrow(request);

            var orderItem = await unitOfWork.Orders.GetOrderItemByIdAsync(request.OrderItemId, ct);
            if (orderItem == null || orderItem.Order.UserId != request.UserId)
                throw new NotFoundException("Order item not found.");

            if (orderItem.Order.Status != OrderStatus.Completed)
                throw new AppValidationException(nameof(request.OrderItemId), "You can only review items from completed orders.");

            var isReviewed = await unitOfWork.Reviews.IsExistByOrderItemIdAsync(request.OrderItemId, ct);
            if (isReviewed)
                throw new AppValidationException(nameof(request.OrderItemId), "This item has already been reviewed.");

            var review = new Review(request.UserId, request.OrderItemId, request.Rating, request.Comment);

            unitOfWork.Reviews.Add(review);
            await unitOfWork.SaveChangesAsync(ct);

            return new CreateReviewResponse
            {
                Id = review.Id,
                OrderItemId = review.OrderItemId,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt
            };
        }
    }
}

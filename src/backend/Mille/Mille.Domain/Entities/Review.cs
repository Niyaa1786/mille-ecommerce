using Mille.Domain.Exceptions;

namespace Mille.Domain.Entities
{
    public class Review
    {
        public const int MinRating = 1;
        public const int MaxRating = 5;

        public int Id { get; private set; }
        public Guid UserId { get; private set; }
        public int OrderItemId { get; private set; }
        public int Rating { get; private set; }
        public string? Comment { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public User User { get; private set; }
        public OrderItem OrderItem { get; private set; }

        private Review() { }

        public Review(Guid userId, int orderItemId, int rating, string? comment)
        {
            if (rating < MinRating || rating > MaxRating)
                throw new DomainException($"Rating must be between {MinRating} and {MaxRating}.");

            UserId = userId;
            OrderItemId = orderItemId;
            Rating = rating;
            Comment = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim();
            CreatedAt = DateTime.UtcNow;
        }
    }
}

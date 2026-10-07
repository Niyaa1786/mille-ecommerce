namespace Mille.Application.Features.Reviews.CreateReview
{
    public class CreateReviewResponse
    {
        public int Id { get; set; }
        public int OrderItemId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Message { get; set; } = "Review submitted successfully.";
    }
}

namespace Mille.Application.Features.Reviews.GetReviewsByProduct
{
    public class GetReviewsByProductResponse
    {
        public double AverageRating { get; set; }
        public List<ReviewDto> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }

    public class ReviewDto
    {
        public int Id { get; set; }
        public string UserFullName { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public string SKU { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}

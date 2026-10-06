namespace Mille.Application.Features.Categories.GetCategories
{
    public class GetCategoriesRequest
    {
        public bool IncludeDeleted { get; set; } = false;
        public int Page {  get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Keyword { get; set; }
    }
}

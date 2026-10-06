using System.Text.Json.Serialization;

namespace Mille.Application.Features.Categories.UpdateCategory
{
    public class UpdateCategoryRequest
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}

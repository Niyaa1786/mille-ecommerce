using System.Text.Json.Serialization;

namespace Mille.Application.Features.Categories.DeleteCategory
{
    public class DeleteCategoryRequest
    {
        [JsonIgnore]
        public int Id { get; set; }
    }
}

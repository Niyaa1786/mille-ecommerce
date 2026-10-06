using System.Text.Json.Serialization;

namespace Mille.Application.Features.Categories.GetCategory
{
    public class GetCategoryRequest
    {
        [JsonIgnore]
        public int Id { get; set; }
    }
}

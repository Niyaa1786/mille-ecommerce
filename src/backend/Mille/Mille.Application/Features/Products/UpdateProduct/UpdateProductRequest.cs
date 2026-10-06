using Microsoft.AspNetCore.Http;
using Mille.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Mille.Application.Features.Products.UpdateProduct
{
    public class UpdateProductRequest
    {
        [JsonIgnore]
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int CategoryId { get; set; }
        public ProductStatus Status { get; set; } = ProductStatus.Active;
        public List<UpdateVariantRequest> Variants { get; set; } = new();
        public List<IFormFile> Images { get; set; } = new();
    }
    public class UpdateVariantRequest
    {
        public string SKU { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }
    }
}

using Microsoft.AspNetCore.Http;
using Mille.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mille.Application.Features.Products.CreateProduct
{
    public class CreateProductRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int CategoryId { get; set; }
        public ProductStatus Status { get; set; } = ProductStatus.Active;
        public List<CreateVariantRequest> Variants { get; set; } = new();
        public List<IFormFile> Images { get; set; } = new();
    }
    public class CreateVariantRequest
    {
        public string SKU { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }
    }
}

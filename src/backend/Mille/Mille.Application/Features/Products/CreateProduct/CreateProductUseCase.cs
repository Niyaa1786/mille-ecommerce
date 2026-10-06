using FluentValidation;
using Mille.Application.Common.Exceptions;
using Mille.Application.Common.Interfaces;
using Mille.Domain.Entities;

namespace Mille.Application.Features.Products.CreateProduct
{
    public class CreateProductUseCase : IUseCase<CreateProductRequest, CreateProductResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileUploadService _fileUploadService;
        private readonly IValidator<CreateProductRequest> _validator;
        public CreateProductUseCase(IUnitOfWork unitOfWork, IFileUploadService fileUploadService, IValidator<CreateProductRequest> validator)
        {
            _unitOfWork = unitOfWork;
            _fileUploadService = fileUploadService;
            _validator = validator;
        }

        public async Task<CreateProductResponse> ExecuteAsync(CreateProductRequest request, CancellationToken ct = default)
        {
            _validator.ValidateAndThrow(request);

            var category = await _unitOfWork.Categories.GetByIdAsync(request.CategoryId, ct);
            if (category is null || category.IsDeleted)
                throw new NotFoundException(nameof(Category), request.CategoryId);

            var skus = request.Variants.Select(s => s.SKU).ToList();
            if (skus.Count != skus.Distinct().Count())
                throw new AppValidationException(nameof(request.Variants), "Duplicated SKU");

            foreach (var sku in skus)
            {
                var isSkuExist = await _unitOfWork.ProductVariants.IsExistBySkuAsync(sku, ct);
                if (isSkuExist)
                    throw new AppValidationException(nameof(CreateVariantRequest.SKU), $"SKU '{sku}' already exists.");
            }

            var product = new Product(request.Name, request.CategoryId, request.Description, request.Status);

            foreach (var variant in request.Variants)
            {
                product.AddVariant(variant.SKU, variant.Price, variant.Stock, variant.Size, variant.Color);
            }

            var uploadResult = await _fileUploadService.UploadFilesAsync(request.Images, "products", ct);
            var isFirst = true;
            foreach(var result in uploadResult)
            {
                product.AddImage(result.Url, result.PublicId, isFirst);
                isFirst = false;
            }

            _unitOfWork.Products.Add(product);
            await _unitOfWork.SaveChangesAsync(ct);

            return new CreateProductResponse
            {
                Id = product.Id,
                CategoryId = product.CategoryId,
                Name = product.Name,
                Description = product.Description,
                Status = product.Status.ToString(),
                Variants = product.Variants.Select(v => new ProductVariantDto
                {
                    Id = v.Id,
                    SKU = v.SKU,
                    Price = v.Price,
                    Stock = v.Stock,
                    Size = v.Size,
                    Color = v.Color,
                    CreatedAt = v.CreatedAt,
                    UpdatedAt = v.UpdatedAt,

                }).ToList(),
                Images = product.Images.Select(i => new ProductImageDto
                {
                    Id = i.Id,
                    ImageUrl = i.ImageUrl,
                    PublicId = i.PublicId,
                    IsThumbnail = i.IsThumbnail,

                }).ToList()
            };
        }
    }
}

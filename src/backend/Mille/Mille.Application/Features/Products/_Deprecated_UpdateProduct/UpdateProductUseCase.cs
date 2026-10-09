using FluentValidation;
using Mille.Application.Common.Exceptions;
using Mille.Application.Common.Interfaces;
using Mille.Application.Features.Products.CreateProduct;

namespace Mille.Application.Features.Products._Deprecated_UpdateProduct
{
    public class UpdateProductUseCase : IUseCase<UpdateProductRequest, UpdateProductResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileUploadService _fileUploadService;
        private readonly IValidator<UpdateProductRequest> _validator;
        public UpdateProductUseCase(IUnitOfWork unitOfWork, IFileUploadService fileUploadService, IValidator<UpdateProductRequest> validator)
        {
            _unitOfWork = unitOfWork;
            _fileUploadService = fileUploadService;
            _validator = validator;
        }

        public async Task<UpdateProductResponse> ExecuteAsync(UpdateProductRequest request, CancellationToken ct = default)
        {
            _validator.ValidateAndThrow(request);

            var product = await _unitOfWork.Products.GetByIdAsync(request.Id, ct);
            if (product == null || product.IsDeleted)
                throw new NotFoundException(nameof(Products), request.Id);

            var category = await _unitOfWork.Categories.GetByIdAsync(request.CategoryId, ct);
            if (category == null || category.IsDeleted)
                throw new NotFoundException(nameof(Categories), request.CategoryId);

            var skus = request.Variants.Select(v => v.SKU).ToList();
            if (skus.Count() != skus.Distinct().Count())
                throw new AppValidationException(nameof(request.Variants), "Duplicate SKU in request.");

            foreach (var sku in skus)
            {
                var existingVariant = await _unitOfWork.ProductVariants.GetBySKUAsync(sku, ct);
                if (existingVariant != null && existingVariant.ProductId != request.Id)
                    throw new AppValidationException(nameof(CreateVariantRequest.SKU), $"SKU '{sku}' already exists.");
            }

            var oldPublicIds = product.GetPublicIds();
            var uploadResults = await _fileUploadService.UploadFilesAsync(request.Images, "products", ct);

            var newVariants = request.Variants.Select(v => (v.SKU, v.Price, v.Stock, v.Size, v.Color)).ToList();
            var newImages = uploadResults.Select(i => (i.Url, i.PublicId)).ToList();

            product.Update(request.Name, request.CategoryId, request.Description, request.Status);

            var oldVariants = product.Variants.ToList();
            foreach (var oldVariant in oldVariants)
            {
                product.RemoveVariant(oldVariant.Id);
                _unitOfWork.ProductVariants.Remove(oldVariant);
            }

            foreach (var v in newVariants)
            {
                var addedVariant = product.AddVariant(v.SKU, v.Price, v.Stock, v.Size, v.Color);
                _unitOfWork.ProductVariants.Add(addedVariant);
            }

            var oldImages = product.Images.ToList();
            foreach (var oldImage in oldImages)
            {
                product.RemoveImage(oldImage.Id);
                _unitOfWork.ProductImages.Remove(oldImage);
            }

            var isFirst = true;
            foreach (var img in newImages)
            {
                var addedImage = product.AddImage(img.Url, img.PublicId, isFirst);
                _unitOfWork.ProductImages.Add(addedImage);
                isFirst = false;
            }

            await _unitOfWork.SaveChangesAsync(ct);

            var isSuccess = await _fileUploadService.DeleteFilesAsync(oldPublicIds, ct);
            if (isSuccess == false)
                throw new Exception($"Failed to delete images on Cloudinary for list: {oldPublicIds}.");

            return new UpdateProductResponse
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
                    PublicId = i.PublicId,
                    ImageUrl = i.ImageUrl,
                    IsThumbnail = i.IsThumbnail
                }).ToList()
            };
        }
    }
}

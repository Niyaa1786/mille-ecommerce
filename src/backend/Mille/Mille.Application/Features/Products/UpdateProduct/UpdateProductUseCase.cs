using Mille.Application.Common.Exceptions;
using Mille.Application.Common.Interfaces;
using Mille.Domain.Entities;
using FluentValidation;

namespace Mille.Application.Features.Products.UpdateProduct
{
    public class UpdateProductUseCase : IUseCase<UpdateProductRequest, UpdateProductResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<UpdateProductRequest> _validator;

        public UpdateProductUseCase(IUnitOfWork unitOfWork, IValidator<UpdateProductRequest> validator)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
        }

        public async Task<UpdateProductResponse> ExecuteAsync(UpdateProductRequest request, CancellationToken ct = default)
        {
            _validator.ValidateAndThrow(request);

            var product = await _unitOfWork.Products.GetByIdWithDetailsAsync(request.Id, ct);
            if (product == null || product.IsDeleted)
                throw new NotFoundException(nameof(Product), request.Id);

            var category = await _unitOfWork.Categories.GetByIdAsync(request.CategoryId, ct);
            if (category == null || category.IsDeleted)
                throw new NotFoundException(nameof(Category), request.CategoryId);

            var skus = request.Variants.Select(v => v.SKU).ToList();
            if (skus.Count != skus.Distinct().Count())
                throw new AppValidationException(nameof(request.Variants), "Duplicate SKU in request.");

            foreach (var sku in skus)
            {
                var existingVariant = await _unitOfWork.ProductVariants.GetBySKUAsync(sku, ct);
                if (existingVariant != null && existingVariant.ProductId != request.Id)
                    throw new AppValidationException(nameof(UpdateVariantRequest.SKU), $"SKU '{sku}' already exists.");
            }

            product.Update(request.Name, request.CategoryId, request.Description, request.Status);

            // Update Variants
            foreach (var oldVariant in product.Variants.ToList())
                product.RemoveVariant(oldVariant.Id);


            foreach (var v in request.Variants)
            {
                var addedVariant = product.AddVariant(v.SKU, v.Price, v.Stock, v.Size, v.Color);
                _unitOfWork.ProductVariants.Add(addedVariant);
            }

            await _unitOfWork.SaveChangesAsync(ct);

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
                    UpdatedAt = v.UpdatedAt
                }).ToList()
            };
        }
    }
}

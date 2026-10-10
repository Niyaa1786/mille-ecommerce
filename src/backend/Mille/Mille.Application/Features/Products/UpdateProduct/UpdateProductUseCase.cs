using FluentValidation;
using Mille.Application.Common.Exceptions;
using Mille.Application.Common.Interfaces;
using Mille.Domain.Entities;

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

            foreach (var v in request.Variants)
                v.SKU = v.SKU.Trim();

            var skus = request.Variants.Select(v => v.SKU).ToList();
            if (skus.Count != skus.Distinct(StringComparer.OrdinalIgnoreCase).Count())
                throw new AppValidationException(nameof(request.Variants), "Duplicate SKU in request.");

            var requestIds = request.Variants.Where(v => v.Id.HasValue).Select(v => v.Id!).ToList();
            if (requestIds.Count != requestIds.Distinct().Count())
                throw new AppValidationException(nameof(request.Variants), "Duplicate variant Id in request.");

            foreach (var sku in skus)
            {
                var existingVariant = await _unitOfWork.ProductVariants.GetBySKUAsync(sku, ct);
                if (existingVariant != null && existingVariant.ProductId != request.Id)
                    throw new AppValidationException(nameof(UpdateVariantRequest.SKU), $"SKU '{sku}' already exists.");
            }

            product.Update(request.Name, request.CategoryId, request.Description, request.Status);

            // 1. Variant not in request => remove (if no order)
            foreach (var variant in product.Variants.ToList())
            {
                if (requestIds.Contains(variant.Id))
                    continue;

                if (await _unitOfWork.ProductVariants.HasOrdersAsync(variant.Id, ct))
                    throw new AppValidationException(nameof(request.Variants), $"Variant '{variant.SKU}' already has orders and cannot be removed.");

                product.RemoveVariant(variant.Id);
            }

            // 2. Variant in request but not in product => add
            foreach (var item in request.Variants)
            {
                if (item.Id == null)
                {
                    var added = product.AddVariant(item.SKU, item.Price, item.Stock, item.Size, item.Color);
                    _unitOfWork.ProductVariants.Add(added);
                    continue;
                }

                var variant = product.Variants.FirstOrDefault(v => v.Id == item.Id);
                if (variant == null)
                    throw new AppValidationException(nameof(request.Variants), "Variant does not belong to this product.");

                // SKU is identifier, not allowed to change. If different SKU is needed, add a new variant.
                if (!string.Equals(variant.SKU, item.SKU, StringComparison.OrdinalIgnoreCase))
                    throw new AppValidationException(nameof(request.Variants), $"SKU of variant '{variant.SKU}' cannot be changed. Add a new variant instead.");

                // If the variant has orders, its size and color cannot be changed. If a different size or color is needed, add a new variant.
                var sizeOrColorChanged = variant.Size != item.Size || variant.Color != item.Color;
                if (sizeOrColorChanged && await _unitOfWork.ProductVariants.HasOrdersAsync(variant.Id, ct))
                    throw new AppValidationException(nameof(request.Variants), $"Variant '{variant.SKU}' already has orders; its size and color cannot be changed. Add a new variant instead.");

                variant.Update(item.Price, item.Stock, item.Size, item.Color);
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
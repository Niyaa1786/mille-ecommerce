using Mille.Application.Common.Exceptions;
using Mille.Application.Common.Interfaces;
using Mille.Domain.Entities;
using FluentValidation;

namespace Mille.Application.Features.Products.UpdateProductImages
{
    public class UpdateProductImagesUseCase : IUseCase<UpdateProductImagesRequest, UpdateProductImagesResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileUploadService _fileUploadService;
        private readonly IValidator<UpdateProductImagesRequest> _validator;

        public UpdateProductImagesUseCase(
            IUnitOfWork unitOfWork,
            IFileUploadService fileUploadService,
            IValidator<UpdateProductImagesRequest> validator)
        {
            _unitOfWork = unitOfWork;
            _fileUploadService = fileUploadService;
            _validator = validator;
        }

        public async Task<UpdateProductImagesResponse> ExecuteAsync(UpdateProductImagesRequest request, CancellationToken ct = default)
        {
            _validator.ValidateAndThrow(request);

            var product = await _unitOfWork.Products.GetByIdWithDetailsAsync(request.ProductId, ct);
            if (product == null || product.IsDeleted)
                throw new NotFoundException(nameof(Product), request.ProductId);

            var oldPublicIds = product.GetPublicIds();

            var uploadResults = (await _fileUploadService.UploadFilesAsync(request.Images, "products", ct)).ToList();

            try
            {
                foreach (var oldImage in product.Images.ToList())
                {
                    product.RemoveImage(oldImage.Id);
                    _unitOfWork.ProductImages.Remove(oldImage);
                }

                var isFirst = true;
                foreach (var result in uploadResults)
                {
                    var addedImage = product.AddImage(result.Url, result.PublicId, isFirst);
                    _unitOfWork.ProductImages.Add(addedImage);
                    isFirst = false;
                }

                await _unitOfWork.SaveChangesAsync(ct);
            }
            catch
            {
                await _fileUploadService.DeleteFilesAsync(uploadResults.Select(r => r.PublicId), CancellationToken.None);
                throw;
            }

            await _fileUploadService.DeleteFilesAsync(oldPublicIds, CancellationToken.None);

            return new UpdateProductImagesResponse
            {
                ProductId = product.Id,
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

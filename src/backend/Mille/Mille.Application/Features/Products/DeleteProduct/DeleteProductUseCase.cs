using Mille.Application.Common.Exceptions;
using Mille.Application.Common.Interfaces;

namespace Mille.Application.Features.Products.DeleteProduct
{
    public class DeleteProductUseCase : IUseCase<DeleteProductRequest, DeleteProductResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileUploadService _fileUploadService;
        public DeleteProductUseCase(IUnitOfWork unitOfWork, IFileUploadService fileUploadService)
        {
            _unitOfWork = unitOfWork;
            _fileUploadService = fileUploadService;
        }

        public async Task<DeleteProductResponse> ExecuteAsync(DeleteProductRequest request, CancellationToken ct = default)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(request.Id, ct);
            if (product == null || product.IsDeleted)
                throw new NotFoundException(nameof(Products), request.Id);

            product.SoftDelete();

            await _unitOfWork.SaveChangesAsync(ct);

            return new DeleteProductResponse();
        }
    }
}

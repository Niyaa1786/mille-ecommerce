using Mille.Application.Common.Exceptions;
using Mille.Application.Common.Interfaces;
using Mille.Domain.Entities;

namespace Mille.Application.Features.Categories.DeleteCategory
{
    public class DeleteCategoryUseCase : IUseCase<DeleteCategoryRequest, DeleteCategoryResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        public DeleteCategoryUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DeleteCategoryResponse> ExecuteAsync(DeleteCategoryRequest request, CancellationToken ct = default)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(request.Id, ct);
            if (category == null || category.IsDeleted)
                throw new NotFoundException(nameof(Category), request.Id);

            //Check is product of this category still active
            var products = await _unitOfWork.Products.GetProductsAsync(categoryId: request.Id, includeDeleted: false, ct: ct);
            if (products.Any())
                throw new AppValidationException(nameof(request.Id), "Cannot delete category with existing products.");

            category.SoftDelete();
            await _unitOfWork.SaveChangesAsync(ct);

            return new DeleteCategoryResponse();
        }
    }
}

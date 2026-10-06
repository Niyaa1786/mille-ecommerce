using FluentValidation;
using Mille.Application.Common.Exceptions;
using Mille.Application.Common.Interfaces;
using Mille.Domain.Entities;

namespace Mille.Application.Features.Categories.UpdateCategory
{
    public class UpdateCategoryUseCase : IUseCase<UpdateCategoryRequest, UpdateCategoryResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<UpdateCategoryRequest> _validator;

        public UpdateCategoryUseCase(IUnitOfWork unitOfWork, IValidator<UpdateCategoryRequest> validator)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
        }

        public async Task<UpdateCategoryResponse> ExecuteAsync(UpdateCategoryRequest request, CancellationToken ct = default)
        {
            _validator.ValidateAndThrow(request);

            var category = await _unitOfWork.Categories.GetByIdAsync(request.Id, ct);
            if (category == null || category.IsDeleted)
                throw new NotFoundException(nameof(Category), request.Id);

            var isExisting = await _unitOfWork.Categories.IsExistByName(request.Name, category.Name, ct);
            if (isExisting)
                throw new AppValidationException(nameof(request.Name), "Category name already exists");

            category.UpdateInfo(request.Name, request.Description!);
            await _unitOfWork.SaveChangesAsync(ct);

            return new UpdateCategoryResponse
            {
                Id = request.Id,
                Name = category.Name,
                Description = category.Description
            };
        }
    }
}

using FluentValidation;
using Mille.Application.Common.Exceptions;
using Mille.Application.Common.Interfaces;
using Mille.Domain.Entities;

namespace Mille.Application.Features.Categories.CreateCategory
{
    public class CreateCategoryUseCase : IUseCase<CreateCategoryRequest, CreateCategoryResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreateCategoryRequest> _validator;
        public CreateCategoryUseCase(IUnitOfWork unitOfWork, IValidator<CreateCategoryRequest> validator)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
        }

        public async Task<CreateCategoryResponse> ExecuteAsync(CreateCategoryRequest request, CancellationToken ct = default)
        {
            _validator.ValidateAndThrow(request);

            var isExisting = await _unitOfWork.Categories.IsExistByName(request.Name,null, ct); 
            if (isExisting)
                throw new AppValidationException(nameof(request.Name), "Category name already exists.");

            var category = new Category(request.Name, request.Description!);
            _unitOfWork.Categories.Add(category);
            await _unitOfWork.SaveChangesAsync(ct);

            return new CreateCategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description!
            };
        }
    }
}

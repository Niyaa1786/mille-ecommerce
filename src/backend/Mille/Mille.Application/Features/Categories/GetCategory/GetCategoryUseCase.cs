using Mille.Application.Common.Exceptions;
using Mille.Application.Common.Interfaces;

namespace Mille.Application.Features.Categories.GetCategory
{
    public class GetCategoryUseCase : IUseCase<GetCategoryRequest, GetCategoryResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetCategoryUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<GetCategoryResponse> ExecuteAsync(GetCategoryRequest request, CancellationToken ct = default)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(request.Id, ct);
            if (category == null || category.IsDeleted)
                throw new NotFoundException("Category", request.Id);

            return new GetCategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt
            };
        }
    }
}

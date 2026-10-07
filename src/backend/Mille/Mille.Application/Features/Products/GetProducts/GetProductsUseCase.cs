using Mille.Application.Common.Interfaces;

namespace Mille.Application.Features.Products.GetProducts
{
    public class GetProductsUseCase : IUseCase<GetProductsRequest, GetProductsResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetProductsUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<GetProductsResponse> ExecuteAsync(GetProductsRequest request, CancellationToken ct = default)
        {
            var product = await _unitOfWork.Products.GetProductsAsync(
                request.CategoryId,
                request.Keyword,
                request.Status,
                request.IncludeDeleted,
                request.Page,
                request.PageSize,
                ct);

            var totalCount = await _unitOfWork.Products.CountProductsAsync(
                request.CategoryId,
                request.Keyword,
                request.Status,
                request.IncludeDeleted,
                ct);

            var items = product.Select(p => new ProductSummaryDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Status = p.Status,
                CategoryName = p.Category?.Name,
                MinPrice = p.GetCheapestPrice(),
                ThumbnailUrl = p.Images.FirstOrDefault(i => i.IsThumbnail)?.ImageUrl ?? p.Images.FirstOrDefault()?.ImageUrl,
                CreatedAt = p.CreatedAt
            }).ToList();

            return new GetProductsResponse
            {
                Items = items,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }
    }
}

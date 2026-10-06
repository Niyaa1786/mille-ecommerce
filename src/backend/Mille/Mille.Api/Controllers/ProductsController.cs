using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mille.Api.Responses;
using Mille.Application.Features.Products.CreateProduct;
using Mille.Application.Features.Products.DeleteProduct;
using Mille.Application.Features.Products.GetProduct;
using Mille.Application.Features.Products.GetProducts;
using Mille.Application.Features.Products.UpdateProduct;
using Mille.Domain.Enums;

namespace Mille.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public class ProductsController : ControllerBase
    {
        #region
        private readonly CreateProductUseCase _createUseCase;
        private readonly UpdateProductUseCase _updateUseCase;
        private readonly DeleteProductUseCase _deleteUseCase;
        private readonly GetProductUseCase _getProductUseCase;
        private readonly GetProductsUseCase _getProductsUseCase;

        public ProductsController(
            CreateProductUseCase createUseCase,
            UpdateProductUseCase updateUseCase,
            DeleteProductUseCase deleteUseCase,
            GetProductUseCase getProductUseCase,
            GetProductsUseCase getProductsUseCase)
        {
            _createUseCase = createUseCase;
            _updateUseCase = updateUseCase;
            _deleteUseCase = deleteUseCase;
            _getProductUseCase = getProductUseCase;
            _getProductsUseCase = getProductsUseCase;
        }
        #endregion

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetProducts([FromQuery] GetProductsRequest request, CancellationToken ct)
        {
            var result = await _getProductsUseCase.ExecuteAsync(request, ct);
            return Ok(ApiResponse<GetProductsResponse>.Success(result));
        }

        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProduct(Guid id, CancellationToken ct)
        {
            var request = new GetProductRequest { Id = id };
            var result = await _getProductUseCase.ExecuteAsync(request, ct);
            return Ok(ApiResponse<GetProductResponse>.Success(result));
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromForm] CreateProductRequest request, CancellationToken ct)
        {
            var result = await _createUseCase.ExecuteAsync(request, ct);
            return Ok(ApiResponse<CreateProductResponse>.Success(result, "Product created."));
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromForm] UpdateProductRequest request, CancellationToken ct)
        {
            request.Id = id;
            var result = await _updateUseCase.ExecuteAsync(request, ct);
            return Ok(ApiResponse<UpdateProductResponse>.Success(result, "Product updated."));
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteProduct(Guid id, CancellationToken ct)
        {
            var request = new DeleteProductRequest { Id = id };
            var result = await _deleteUseCase.ExecuteAsync(request, ct);
            return Ok(ApiResponse<DeleteProductResponse>.Success(result, result.Message));
        }
    }
}


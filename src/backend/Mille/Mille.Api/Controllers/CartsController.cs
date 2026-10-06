using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mille.Api.Responses;
using Mille.Application.Features.Carts.AddToCart;
using Mille.Application.Features.Carts.GetCart;
using Mille.Application.Features.Carts.RemoveCartItem;
using Mille.Application.Features.Carts.UpdateCartItem;
using System.Security.Claims;

namespace Mille.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartsController : ControllerBase
    {
        #region
                private readonly GetCartUseCase _getCartUseCase;
                private readonly AddToCartUseCase _addToCartUseCase;
                private readonly UpdateCartItemUseCase _updateCartItemUseCase;
                private readonly RemoveCartItemUseCase _removeCartItemUseCase;
                public CartsController(
                    GetCartUseCase getCartUseCase,
                    AddToCartUseCase addToCartUseCase,
                    UpdateCartItemUseCase updateCartItemUseCase,
                    RemoveCartItemUseCase removeCartItemUseCase)
                {
                    _getCartUseCase = getCartUseCase;
                    _addToCartUseCase = addToCartUseCase;
                    _updateCartItemUseCase = updateCartItemUseCase;
                    _removeCartItemUseCase = removeCartItemUseCase;
                }
        #endregion
        [HttpGet]
        public async Task<IActionResult> GetCart(CancellationToken ct)
        {   
            var userId = GetUserId();
            var request = new GetCartRequest { UserId = userId };
            var result = await _getCartUseCase.ExecuteAsync(request, ct);

            return Ok(ApiResponse<GetCartResponse>.Success(result));
        }

        [HttpPost("items")]
        public async Task<IActionResult> AddToCart(AddToCartRequest request, CancellationToken ct)
        {
            request.UserId = GetUserId();
            var result = await _addToCartUseCase.ExecuteAsync(request, ct);

            return Ok(ApiResponse<AddToCartResponse>.Success(result, result.Message));
        }

        [HttpPut("items/{cartItemId}")]
        public async Task<IActionResult> UpdateCartItem(int cartItemId, UpdateCartItemRequest request, CancellationToken ct)
        {
            request.UserId = GetUserId();
            request.CartItemId = cartItemId;
            var result = await _updateCartItemUseCase.ExecuteAsync(request, ct);

            return Ok(ApiResponse<UpdateCartItemResponse>.Success(result, "Cart item updated."));
        }

        [HttpDelete("items/{cartItemId}")]
        public async Task<IActionResult> RemoveCartItem(int cartItemId, CancellationToken ct)
        {
            var request = new RemoveCartItemRequest
            {
                UserId = GetUserId(),
                CartItemId = cartItemId
            };
            var result = await _removeCartItemUseCase.ExecuteAsync(request, ct);

            return Ok(ApiResponse<RemoveCartItemResponse>.Success(result, result.Message));
        }


        private Guid GetUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User ID claim not found.");
            return Guid.Parse(userId);
        }
    }
}

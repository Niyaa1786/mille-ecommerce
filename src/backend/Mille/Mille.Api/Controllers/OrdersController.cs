using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mille.Api.Responses;
using Mille.Application.Features.Orders.CancelOrder;
using Mille.Application.Features.Orders.ConfirmPayment;
using Mille.Application.Features.Orders.CreateOrder;
using Mille.Application.Features.Orders.GetOrder;
using Mille.Application.Features.Orders.GetOrders;
using Mille.Application.Features.Orders.GetOrdersByUserId;
using Mille.Application.Features.Orders.UpdateOrderStatus;
using Mille.Domain.Enums;
using System.Security.Claims;

namespace Mille.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        #region
        private readonly CreateOrderUseCase _createOrderUseCase;
        private readonly GetOrdersUseCase _getOrdersUseCase;
        private readonly GetOrdersByUserIdUseCase _getOrdersByUserIdUseCase;
        private readonly GetOrderUseCase _getOrderUseCase;
        private readonly CancelOrderUseCase _cancelOrderUseCase;
        private readonly UpdateOrderStatusUseCase _updateOrderStatusUseCase;
        private readonly ConfirmPaymentUseCase _confirmPaymentUseCase;

        public OrdersController(
            CreateOrderUseCase createOrderUseCase,
            GetOrdersUseCase getOrdersUseCase,
            GetOrdersByUserIdUseCase getOrdersByUserIdUseCase,
            GetOrderUseCase getOrderUseCase,
            CancelOrderUseCase cancelOrderUseCase,
            UpdateOrderStatusUseCase updateOrderStatusUseCase,
            ConfirmPaymentUseCase confirmPaymentUseCase)
        {
            _createOrderUseCase = createOrderUseCase;
            _getOrdersUseCase = getOrdersUseCase;
            _getOrdersByUserIdUseCase = getOrdersByUserIdUseCase;
            _getOrderUseCase = getOrderUseCase;
            _cancelOrderUseCase = cancelOrderUseCase;
            _updateOrderStatusUseCase = updateOrderStatusUseCase;
            _confirmPaymentUseCase = confirmPaymentUseCase;
        }
        #endregion
        [HttpPost]
        public async Task<IActionResult> CreateOrder(CreateOrderRequest request, CancellationToken ct)
        {
            request.UserId = GetUserId();
            var result = await _createOrderUseCase.ExecuteAsync(request, ct);

            return Ok(ApiResponse<CreateOrderResponse>.Success(result, result.Message));
        }

        [HttpGet("my-orders")]
        public async Task<IActionResult> GetMyOrders([FromQuery] GetOrdersByUserIdRequest request, CancellationToken ct)
        {
            request.UserId = GetUserId();
            var result = await _getOrdersByUserIdUseCase.ExecuteAsync(request, ct);

            return Ok(ApiResponse<GetOrdersByUserIdResponse>.Success(result));
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetOrder(Guid id, CancellationToken ct)
        {
            var request = new GetOrderRequest { OrderId = id, UserId = GetUserId() };
            var result = await _getOrderUseCase.ExecuteAsync(request, ct);

            return Ok(ApiResponse<GetOrderResponse>.Success(result));
        }

        [HttpPost("{id:guid}/cancel")]
        public async Task<IActionResult> CancelOrder(Guid id, CancellationToken ct)
        {
            var request = new CancelOrderRequest { OrderId = id, UserId = GetUserId() };
            var result = await _cancelOrderUseCase.ExecuteAsync(request, ct);

            return Ok(ApiResponse<CancelOrderResponse>.Success(result, result.Message));
        }

        
        [HttpGet]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> GetAllOrders([FromQuery] GetOrdersRequest request, CancellationToken ct)
        {
            var result = await _getOrdersUseCase.ExecuteAsync(request, ct);

            return Ok(ApiResponse<GetOrdersResponse>.Success(result));
        }

        [HttpPut("{id:guid}/status")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> UpdateOrderStatus(Guid id, UpdateOrderStatusRequest request, CancellationToken ct)
        {
            request.OrderId = id;
            var result = await _updateOrderStatusUseCase.ExecuteAsync(request, ct);

            return Ok(ApiResponse<UpdateOrderStatusResponse>.Success(result, result.Message));
        }

        [HttpPost("{id:guid}/confirm-payment")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> ConfirmPayment(Guid id, CancellationToken ct)
        {
            var request = new ConfirmPaymentRequest { OrderId = id };
            var result = await _confirmPaymentUseCase.ExecuteAsync(request, ct);

            return Ok(ApiResponse<ConfirmPaymentResponse>.Success(result, result.Message));
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

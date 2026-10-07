using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mille.Api.Responses;
using Mille.Application.Features.Coupons.CreateCoupon;
using Mille.Application.Features.Coupons.DeleteCoupon;
using Mille.Application.Features.Coupons.GetCoupon;
using Mille.Application.Features.Coupons.GetCoupons;
using Mille.Application.Features.Coupons.UpdateCoupon;
using Mille.Application.Features.Coupons.ValidateCoupon;
using Mille.Domain.Enums;
using System.Security.Claims;

namespace Mille.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CouponsController : ControllerBase
    {
        #region
        private readonly CreateCouponUseCase _createUseCase;
        private readonly UpdateCouponUseCase _updateUseCase;
        private readonly DeleteCouponUseCase _deleteUseCase;
        private readonly GetCouponUseCase _getCouponUseCase;
        private readonly GetCouponsUseCase _getCouponsUseCase;
        private readonly ValidateCouponUseCase _validateCouponUseCase;

        public CouponsController(
            CreateCouponUseCase createUseCase,
            UpdateCouponUseCase updateUseCase,
            DeleteCouponUseCase deleteUseCase,
            GetCouponUseCase getCouponUseCase,
            GetCouponsUseCase getCouponsUseCase,
            ValidateCouponUseCase validateCouponUseCase)
        {
            _createUseCase = createUseCase;
            _updateUseCase = updateUseCase;
            _deleteUseCase = deleteUseCase;
            _getCouponUseCase = getCouponUseCase;
            _getCouponsUseCase = getCouponsUseCase;
            _validateCouponUseCase = validateCouponUseCase;
        }
        #endregion

        [HttpPost("validate")]
        public async Task<IActionResult> ValidateCoupon(ValidateCouponRequest request, CancellationToken ct)
        {
            request.UserId = GetUserId();
            var result = await _validateCouponUseCase.ExecuteAsync(request, ct);

            return Ok(ApiResponse<ValidateCouponResponse>.Success(result, result.Message));
        }

        [HttpGet]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> GetCoupons([FromQuery] GetCouponsRequest request, CancellationToken ct)
        {
            var result = await _getCouponsUseCase.ExecuteAsync(request, ct);

            return Ok(ApiResponse<GetCouponsResponse>.Success(result));
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> GetCoupon(int id, CancellationToken ct)
        {
            var request = new GetCouponRequest { Id = id };
            var result = await _getCouponUseCase.ExecuteAsync(request, ct);

            return Ok(ApiResponse<GetCouponResponse>.Success(result));
        }

        [HttpPost]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> CreateCoupon(CreateCouponRequest request, CancellationToken ct)
        {
            var result = await _createUseCase.ExecuteAsync(request, ct);

            return Ok(ApiResponse<CreateCouponResponse>.Success(result, "Coupon created."));
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> UpdateCoupon(int id, UpdateCouponRequest request, CancellationToken ct)
        {
            request.Id = id;
            var result = await _updateUseCase.ExecuteAsync(request, ct);

            return Ok(ApiResponse<UpdateCouponResponse>.Success(result, "Coupon updated."));
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> DeleteCoupon(int id, CancellationToken ct)
        {
            var request = new DeleteCouponRequest { Id = id };
            var result = await _deleteUseCase.ExecuteAsync(request, ct);

            return Ok(ApiResponse<DeleteCouponResponse>.Success(result, result.Message));
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

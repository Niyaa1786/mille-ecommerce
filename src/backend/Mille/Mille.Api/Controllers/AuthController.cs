using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mille.Api.Responses;
using Mille.Application.Features.Auth.ChangePassword;
using Mille.Application.Features.Auth.Login;
using Mille.Application.Features.Auth.Logout;
using Mille.Application.Features.Auth.RefreshToken;
using Mille.Application.Features.Auth.Register;
using System.Security.Claims;

namespace Mille.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        #region
        private readonly RegisterUseCase _registerUseCase;
        private readonly LoginUseCase _loginUseCase;
        private readonly RefreshTokenUseCase _refreshTokenUseCase;
        private readonly LogoutUseCase _logoutUseCase;
        private readonly ChangePasswordUseCase _changePasswordUseCase;
        public AuthController(
            RegisterUseCase registerUseCase,
            LoginUseCase loginUseCase,
            RefreshTokenUseCase refreshTokenUseCase,
            LogoutUseCase logoutUseCase,
            ChangePasswordUseCase changePasswordUseCase)
        {
            _registerUseCase = registerUseCase;
            _loginUseCase = loginUseCase;
            _refreshTokenUseCase = refreshTokenUseCase;
            _logoutUseCase = logoutUseCase;
            _changePasswordUseCase = changePasswordUseCase;
        }

        #endregion
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request, CancellationToken ct)
        {
            var result = await _registerUseCase.ExecuteAsync(request, ct);
            var res = ApiResponse<RegisterResponse>.Success(result, "Registration successful.");

            return Ok(res);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request, CancellationToken ct)
        {
            var result = await _loginUseCase.ExecuteAsync(request, ct);
            var res = ApiResponse<LoginResponse>.Success(result, "Login successful.");

            return Ok(res);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequest request, CancellationToken ct)
        {
            var result = await _refreshTokenUseCase.ExecuteAsync(request, ct);
            var res = ApiResponse<RefreshTokenResponse>.Success(result, "Token refreshed.");

            return Ok(res);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(CancellationToken ct)
        {
            var userId = GetUserId();
            var request = new LogoutRequest { UserId = userId };
            var result = await _logoutUseCase.ExecuteAsync(request, ct);
            var res = ApiResponse<LogoutResponse>.Success(result, result.Message);

            return Ok(res);
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest request, CancellationToken ct)
        {
            request.UserId = GetUserId();
            var result = await _changePasswordUseCase.ExecuteAsync(request, ct);
            var res = ApiResponse<ChangePasswordResponse>.Success(result, result.Message);

            return Ok(res);
        }

        private Guid GetUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User ID claim not found");
            return Guid.Parse(userId);
        }
    }
}

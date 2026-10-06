using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mille.Api.Responses;
using System.Security.Claims;
using Mille.Application.Features.Users.AddAddress;
using Mille.Application.Features.Users.DeleteAddress;
using Mille.Application.Features.Users.GetProfile;
using Mille.Application.Features.Users.UpdateAddress;
using Mille.Application.Features.Users.UpdateProfile;
using Mille.Application.Features.Users.UploadAvatar;

namespace Mille.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        #region 
        private readonly GetProfileUseCase _getProfileUseCase;
        private readonly UpdateProfileUseCase _updateProfileUseCase;
        private readonly AddAddressUseCase _addAddressUseCase;
        private readonly UpdateAddressUseCase _updateAddressUseCase;
        private readonly DeleteAddressUseCase _deleteAddressUseCase;
        private readonly UploadAvatarUseCase _uploadAvatarUseCase;
        public UsersController(
            GetProfileUseCase getProfileUseCase,
            UpdateProfileUseCase updateProfileUseCase,
            AddAddressUseCase addAddressUseCase,
            UpdateAddressUseCase updateAddressUseCase,
            DeleteAddressUseCase deleteAddressUseCase,
            UploadAvatarUseCase uploadAvatarUseCase)
        {
            _getProfileUseCase = getProfileUseCase;
            _updateProfileUseCase = updateProfileUseCase;
            _addAddressUseCase = addAddressUseCase;
            _updateAddressUseCase = updateAddressUseCase;
            _deleteAddressUseCase = deleteAddressUseCase;
            _uploadAvatarUseCase = uploadAvatarUseCase;
        }
        #endregion
        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile(CancellationToken ct)
        {
            var userId = GetUserId();
            var request = new GetProfileRequest { UserId = userId };
            var result = await _getProfileUseCase.ExecuteAsync(request, ct);
            var res = ApiResponse<GetProfileResponse>.Success(result, "Profile retrieved.");

            return Ok(res);
        }

        [Authorize]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile(UpdateProfileRequest request, CancellationToken ct)
        {
            request.UserId = GetUserId();
            var result = await _updateProfileUseCase.ExecuteAsync(request, ct);
            var res = ApiResponse<UpdateProfileResponse>.Success(result, "Profile updated.");

            return Ok(res);
        }

        [Authorize]
        [HttpPost("avatar")]
        public async Task<IActionResult> UploadAvatar(IFormFile file, CancellationToken ct)
        {
            var userId = GetUserId();
            using var stream = file.OpenReadStream();
            var request = new UploadAvatarRequest
            {
                UserId = userId,
                FileStream = stream,
                FileName = file.FileName
            };
            var result = await _uploadAvatarUseCase.ExecuteAsync(request, ct);
            var res = ApiResponse<UploadAvatarResponse>.Success(result, "Avatar uploaded.");

            return Ok(res);
        }

        [Authorize]
        [HttpPost("addresses")]
        public async Task<IActionResult> AddAddress(AddAddressRequest request, CancellationToken ct)
        {
            request.UserId = GetUserId();
            var result = await _addAddressUseCase.ExecuteAsync(request, ct);
            var res = ApiResponse<AddAddressResponse>.Success(result, "Address added.");

            return Ok(res);
        }

        [Authorize]
        [HttpPut("addresses/{addressId}")]
        public async Task<IActionResult> UpdateAddress(int addressId, UpdateAddressRequest request, CancellationToken ct)
        {
            request.UserId = GetUserId();
            request.Id = addressId;
            var result = await _updateAddressUseCase.ExecuteAsync(request, ct);
            var res = ApiResponse<UpdateAddressResponse>.Success(result, "Address updated.");

            return Ok(res);
        }

        [Authorize]
        [HttpDelete("addresses/{addressId}")]
        public async Task<IActionResult> DeleteAddress(int addressId, CancellationToken ct)
        {
            var request = new DeleteAddressRequest
            {
                UserId = GetUserId(),
                AddressId = addressId
            };
            var result = await _deleteAddressUseCase.ExecuteAsync(request, ct);
            var res = ApiResponse<DeleteAddressResponse>.Success(result, "Address deleted.");

            return Ok(res);
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


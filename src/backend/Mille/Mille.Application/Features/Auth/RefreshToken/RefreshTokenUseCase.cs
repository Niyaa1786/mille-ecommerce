using FluentValidation;
using Mille.Application.Common.Exceptions;
using Mille.Application.Common.Interfaces;

namespace Mille.Application.Features.Auth.RefreshToken
{
    public class RefreshTokenUseCase : IUseCase<RefreshTokenRequest, RefreshTokenResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IValidator<RefreshTokenRequest> _validator;

        public RefreshTokenUseCase(IUnitOfWork unitOfWork, ITokenGenerator tokenGenerator, IValidator<RefreshTokenRequest> validator)
        {
            _unitOfWork = unitOfWork;
            _tokenGenerator = tokenGenerator;
            _validator = validator;
        }

        public async Task<RefreshTokenResponse> ExecuteAsync(RefreshTokenRequest request, CancellationToken ct = default)
        {
            _validator.ValidateAndThrow(request);

            var user = await _unitOfWork.Users.GetByRefreshTokenAsync(request.RefreshToken, ct);
            if (user is null)
                throw new AppValidationException(nameof(request.RefreshToken), "Invalid refresh token.");

            if (user.RefreshTokenExpiryTime < DateTime.UtcNow)
                throw new AppValidationException(nameof(request.RefreshToken), "Refresh token expired.");

            var tokenResult = _tokenGenerator.GenerateToken(user);

            user.SetRefreshToken(tokenResult.RefreshToken, tokenResult.RefreshTokenExpiration);
            await _unitOfWork.SaveChangesAsync(ct);

            return new RefreshTokenResponse
            {
                AccessToken = tokenResult.AccessToken,
                RefreshToken = tokenResult.RefreshToken,
                AccessTokenExpiration = tokenResult.AccessTokenExpiration,
                RefreshTokenExpiration = tokenResult.RefreshTokenExpiration,
            };
        }
    }
}

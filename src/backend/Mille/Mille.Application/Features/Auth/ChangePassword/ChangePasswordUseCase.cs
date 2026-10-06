using FluentValidation;
using Mille.Application.Common.Exceptions;
using Mille.Application.Common.Interfaces;

namespace Mille.Application.Features.Auth.ChangePassword
{
    public class ChangePasswordUseCase : IUseCase<ChangePasswordRequest, ChangePasswordResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IValidator<ChangePasswordRequest> _validator;

        public ChangePasswordUseCase(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher, IValidator<ChangePasswordRequest> validator)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _validator = validator;
        }
        public async Task<ChangePasswordResponse> ExecuteAsync(ChangePasswordRequest request, CancellationToken ct = default)
        {
            _validator.ValidateAndThrow(request);

            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, ct);
            if (user is null)
                throw new NotFoundException("User not found.");

            var isValidPassword = _passwordHasher.VerifyPassword(request.OldPassword, user.PasswordHash);
            if (!isValidPassword)
                throw new AppValidationException(nameof(request.OldPassword), "Old Password is incorrect");

            var newPasswordHash = _passwordHasher.HashPassword(request.NewPassword);
            user.ChangePassword(newPasswordHash);

            await _unitOfWork.SaveChangesAsync(ct);

            return new ChangePasswordResponse();
        }
    }
}

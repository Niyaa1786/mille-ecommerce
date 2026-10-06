using FluentValidation;
using Mille.Application.Common.Exceptions;
using Mille.Application.Common.Interfaces;
using Mille.Domain.Enums;
using Mille.Domain.Entities;

namespace Mille.Application.Features.Auth.Register
{
    public class RegisterUseCase : IUseCase<RegisterRequest, RegisterResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IValidator<RegisterRequest> _validator;

        public RegisterUseCase(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher, IValidator<RegisterRequest> validator)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _validator = validator;
        }

        public async Task<RegisterResponse> ExecuteAsync(RegisterRequest request, CancellationToken ct = default)
        {
            _validator.ValidateAndThrow(request);

            var existingUser = await _unitOfWork.Users.GetByEmailAsync(request.Email, ct);
            if (existingUser != null)
                throw new AppValidationException(nameof(request.Email), "Email already registered");

            var passwordHash = _passwordHasher.HashPassword(request.Password);

            var user = new User(request.FullName, request.Email, passwordHash, UserRole.Customer);

            if (!string.IsNullOrEmpty(request.Phone))
                user.UpdateProfile(request.FullName, request.Phone);

            _unitOfWork.Users.Add(user);
            await _unitOfWork.SaveChangesAsync(ct);

            return new RegisterResponse
            {
                UserId = user.Id,
                Email = user.Email,
                FullName = user.FullName
            };
        }
    }
}

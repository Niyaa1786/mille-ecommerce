using Mille.Application.Common.Exceptions;
using Mille.Application.Common.Interfaces;

namespace Mille.Application.Features.Auth.Logout
{
    public class LogoutUseCase : IUseCase<LogoutRequest, LogoutResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public LogoutUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<LogoutResponse> ExecuteAsync(LogoutRequest request, CancellationToken ct = default)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, ct);
            if (user == null)
                throw new NotFoundException("User not found.");

            user.RevokeRefreshToken();
            await _unitOfWork.SaveChangesAsync(ct);

            return new LogoutResponse
            {
                Message = "Logged out successfully."
            };
        }
    }
}

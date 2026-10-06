using Mille.Application.Common.DTOs;
using Mille.Application.Common.Exceptions;
using Mille.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mille.Application.Features.Users.GetProfile
{
    public class GetProfileUseCase : IUseCase<GetProfileRequest, GetProfileResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetProfileUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<GetProfileResponse> ExecuteAsync(GetProfileRequest request, CancellationToken ct = default)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, ct);
            if (user == null)
                throw new NotFoundException("User not found.");

            return new GetProfileResponse
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                AvatarUrl = user.AvatarUrl,
                Role = user.Role.ToString(),
                Addresses = user.Addresses.Select(a => new AddressDto
                {
                    Id = a.Id,
                    ReceiverName = a.ReceiverName,
                    ReceiverPhone = a.ReceiverPhone,
                    AddressLine = a.AddressLine,
                    IsDefault = a.IsDefault
                }).ToList()
            };
        }
    }
}

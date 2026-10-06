using FluentValidation;
using Mille.Application.Common.DTOs;
using Mille.Application.Common.Exceptions;
using Mille.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mille.Application.Features.Users.UpdateProfile
{
    public class UpdateProfileUseCase : IUseCase<UpdateProfileRequest, UpdateProfileResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<UpdateProfileRequest> _validator;

        public UpdateProfileUseCase(IUnitOfWork unitOfWork, IValidator<UpdateProfileRequest> validator)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
        }

        public async Task<UpdateProfileResponse> ExecuteAsync(UpdateProfileRequest request, CancellationToken ct = default)
        {
            _validator.ValidateAndThrow(request);

            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, ct);
            if (user == null)
                throw new NotFoundException("User not found.");

            user.UpdateProfile(request.FullName, request.Phone);
            await _unitOfWork.SaveChangesAsync(ct);

            return new UpdateProfileResponse
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

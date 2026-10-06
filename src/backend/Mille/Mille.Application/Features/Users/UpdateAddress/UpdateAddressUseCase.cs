using FluentValidation;
using Mille.Application.Common.Exceptions;
using Mille.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mille.Application.Features.Users.UpdateAddress
{
    public class UpdateAddressUseCase : IUseCase<UpdateAddressRequest, UpdateAddressResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<UpdateAddressRequest> _validator;

        public UpdateAddressUseCase(IUnitOfWork unitOfWork, IValidator<UpdateAddressRequest> validator)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
        }

        public async Task<UpdateAddressResponse> ExecuteAsync(UpdateAddressRequest request, CancellationToken ct = default)
        {
            _validator.ValidateAndThrow(request);

            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, ct);
            if (user == null)
                throw new NotFoundException("User not found.");

            user.UpdateAddress(request.Id, request.ReceiverName, request.ReceiverPhone, request.AddressLine, request.IsDefault);
            await _unitOfWork.SaveChangesAsync(ct);

            return new UpdateAddressResponse();
        }
    }
}

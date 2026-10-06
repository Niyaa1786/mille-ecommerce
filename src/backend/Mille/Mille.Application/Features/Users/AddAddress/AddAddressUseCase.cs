using FluentValidation;
using Mille.Application.Common.Exceptions;
using Mille.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mille.Application.Features.Users.AddAddress
{
    public class AddAddressUseCase : IUseCase<AddAddressRequest, AddAddressResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<AddAddressRequest> _validator;

        public AddAddressUseCase(IUnitOfWork unitOfWork, IValidator<AddAddressRequest> validator)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
        }

        public async Task<AddAddressResponse> ExecuteAsync(AddAddressRequest request, CancellationToken ct = default)
        {
            _validator.ValidateAndThrow(request);

            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, ct);
            if (user == null)
                throw new NotFoundException("User not found.");

            var address = user.AddAddress(request.ReceiverName, request.ReceiverPhone, request.AddressLine, request.IsDefault);
            await _unitOfWork.SaveChangesAsync(ct);

            return new AddAddressResponse
            {
                AddressId = address.Id
            };
        }
    }
}

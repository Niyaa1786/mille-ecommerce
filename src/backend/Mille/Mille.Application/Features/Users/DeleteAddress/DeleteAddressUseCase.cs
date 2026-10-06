using FluentValidation;
using Mille.Application.Common.Exceptions;
using Mille.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mille.Application.Features.Users.DeleteAddress
{
    public class DeleteAddressUseCase : IUseCase<DeleteAddressRequest, DeleteAddressResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<DeleteAddressRequest> _validator;

        public DeleteAddressUseCase(IUnitOfWork unitOfWork, IValidator<DeleteAddressRequest> validator)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
        }

        public async Task<DeleteAddressResponse> ExecuteAsync(DeleteAddressRequest request, CancellationToken ct = default)
        {
            _validator.ValidateAndThrow(request);

            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, ct);
            if (user == null)
                throw new NotFoundException("User not found.");

            user.RemoveAddress(request.AddressId);
            await _unitOfWork.SaveChangesAsync(ct);

            return new DeleteAddressResponse();
        }
    }
}

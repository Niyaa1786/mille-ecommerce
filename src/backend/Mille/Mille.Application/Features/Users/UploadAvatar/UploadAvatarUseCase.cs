using FluentValidation;
using Mille.Application.Common.Exceptions;
using Mille.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mille.Application.Features.Users.UploadAvatar
{
    public class UploadAvatarUseCase : IUseCase<UploadAvatarRequest, UploadAvatarResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileUploadService _fileUploadService;

        public UploadAvatarUseCase(IUnitOfWork unitOfWork, IFileUploadService fileUploadService)
        {
            _unitOfWork = unitOfWork;
            _fileUploadService = fileUploadService;
        }

        public async Task<UploadAvatarResponse> ExecuteAsync(UploadAvatarRequest request, CancellationToken ct = default)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, ct);
            if (user == null)
                throw new NotFoundException("User not found.");

            var folderName = "avatars";
            var publicId = request.UserId.ToString();
            var overWrite = true;

            var avatarUrl = await _fileUploadService.UploadFileAsync(request.FileStream, request.FileName, folderName, publicId, overWrite, ct);

            user.UpdateAvatar(avatarUrl.Url);
            await _unitOfWork.SaveChangesAsync(ct);

            return new UploadAvatarResponse
            {
                AvatarUrl = avatarUrl.Url
            };
        }
    }
}

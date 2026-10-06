namespace Mille.Application.Features.Users.UploadAvatar
{
    public class UploadAvatarRequest
    {
        public Guid UserId { get; set; }
        public Stream FileStream { get; set; } = null!;
        public string FileName { get; set; } = string.Empty;
    }
}

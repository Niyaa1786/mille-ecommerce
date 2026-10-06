using System.Text.Json.Serialization;

namespace Mille.Application.Features.Auth.ChangePassword
{
    public class ChangePasswordRequest
    {
        [JsonIgnore]
        public Guid UserId { get; set; }
        public string OldPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}

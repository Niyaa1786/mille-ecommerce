using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Mille.Application.Features.Users.UpdateProfile
{
    public class UpdateProfileRequest
    {
        [JsonIgnore]
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Phone { get; set; }
    }
}

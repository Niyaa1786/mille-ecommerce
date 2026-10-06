using Mille.Application.Common.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mille.Application.Features.Users.GetProfile
{
    public class GetProfileResponse
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? AvatarUrl { get; set; }
        public string Role { get; set; } = string.Empty;
        public List<AddressDto> Addresses { get; set; } = new();
    }
    public class AddressDto
    {
        public int Id { get; set; }
        public string ReceiverName { get; set; } = string.Empty;
        public string ReceiverPhone { get; set; } = string.Empty;
        public string AddressLine { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
    }
}

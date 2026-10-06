using Mille.Application.Common.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Mille.Application.Features.Users.UpdateAddress
{
    public class UpdateAddressRequest
    {
        [JsonIgnore]
        public Guid UserId { get; set; }
        [JsonIgnore]
        public int Id { get; set; }
        public string ReceiverName { get; set; } = string.Empty;
        public string ReceiverPhone { get; set; } = string.Empty;
        public string AddressLine { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
    }
}

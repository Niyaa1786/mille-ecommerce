using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Mille.Application.Features.Users.AddAddress
{
    public class AddAddressRequest
    {
        [JsonIgnore]
        public Guid UserId { get; set; }
        public string ReceiverName { get; set; } = string.Empty;
        public string ReceiverPhone { get; set; } = string.Empty;
        public string AddressLine { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
    }
}

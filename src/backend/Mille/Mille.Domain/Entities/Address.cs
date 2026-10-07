using Mille.Domain.Exceptions;

namespace Mille.Domain.Entities
{
    public class Address
    {
        public int Id { get; private set; }
        public Guid UserId { get; private set; }
        public string ReceiverName { get; private set; } = string.Empty;
        public string ReceiverPhone { get; private set; } = string.Empty;
        public string AddressLine { get; private set; } = string.Empty;
        public bool IsDefault { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public User User { get; private set; }

        private Address() { }

        public Address(string receiverName, string receiverPhone, string addressLine, bool isDefault)
        {
            ValidateRules(receiverName, receiverPhone, addressLine);

            ReceiverName = receiverName;
            ReceiverPhone = receiverPhone;
            AddressLine = addressLine;
            IsDefault = isDefault;
            CreatedAt = DateTime.UtcNow;
        }

        public void Update(string receiverName, string receiverPhone, string addressLine, bool isDefault)
        {
            ValidateRules(receiverName, receiverPhone, addressLine);

            ReceiverName = receiverName;
            ReceiverPhone = receiverPhone;
            AddressLine = addressLine;
            IsDefault = isDefault;
        }

        public void ClearDefault() => IsDefault = false;

        private static void ValidateRules(string receiverName, string receiverPhone, string addressLine)
        {
            if (string.IsNullOrWhiteSpace(receiverName))
                throw new DomainException("Receiver name is required.");

            if (string.IsNullOrWhiteSpace(receiverPhone))
                throw new DomainException("Receiver phone is required.");

            if (string.IsNullOrWhiteSpace(addressLine))
                throw new DomainException("Address line is required.");
        }
    }
}
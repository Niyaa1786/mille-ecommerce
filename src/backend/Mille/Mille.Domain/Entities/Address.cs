namespace Mille.Domain.Entities
{
    public class Address
    {
        public int Id { get; private set; }
        public Guid UserId { get; private set; }
        public string ReceiverName { get; private set; }
        public string ReceiverPhone { get; private set; }
        public string AddressLine { get; private set; }
        public bool IsDefault { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public User User { get; private set; }

        private Address() { }

        public Address(string receiverName, string receiverPhone, string addressLine, bool isDefault)
        {
            ReceiverName = receiverName;
            ReceiverPhone = receiverPhone;
            AddressLine = addressLine;
            IsDefault = isDefault;
            CreatedAt = DateTime.UtcNow;
        }

        public void Update(string receiverName, string receiverPhone, string addressLine, bool isDefault)
        {
            ReceiverName = receiverName;
            ReceiverPhone = receiverPhone;
            AddressLine = addressLine;
            IsDefault = isDefault;
        }

        public void ClearDefault() => IsDefault = false;

    }
}

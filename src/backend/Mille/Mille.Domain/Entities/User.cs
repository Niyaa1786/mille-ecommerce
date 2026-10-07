using Mille.Domain.Enums;
using Mille.Domain.Exceptions;
using System.Data;

namespace Mille.Domain.Entities
{
    public class User
    {
        public Guid Id { get; private set; }
        public UserRole Role { get; private set; }
        public string FullName { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string PasswordHash { get; private set; } = string.Empty;
        public string? Phone { get; private set; }
        public string? AvatarUrl { get; private set; }
        public string? RefreshToken { get; private set; }
        public DateTime? RefreshTokenExpiryTime { get; private set; }
        public bool IsDeleted { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        private readonly List<Address> _addresses = new();
        public IReadOnlyCollection<Address> Addresses => _addresses.AsReadOnly();

        private User() { }

        public User(string fullName, string email, string passwordHash, UserRole role = UserRole.Customer)
        {
            ValidateRules(fullName, email, passwordHash, role);

            Id = Guid.NewGuid();
            FullName = fullName;
            Email = email;
            PasswordHash = passwordHash;
            Role = role;
            IsDeleted = false;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateProfile(string fullName, string? phone)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new DomainException("Full name is required.");

            FullName = fullName;
            Phone = phone;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateAvatar(string avatarUrl)
        {
            if (string.IsNullOrWhiteSpace(avatarUrl))
                throw new DomainException("Avatar url is required.");

            AvatarUrl = avatarUrl;
            UpdatedAt = DateTime.UtcNow;
        }


        public void ChangePassword(string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new DomainException("Password hash is required.");

            PasswordHash = passwordHash;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetRefreshToken(string token, DateTime expiry)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new DomainException("Refresh token is required.");

            if (expiry <= DateTime.UtcNow)
                throw new DomainException("Refresh token expiry must be in the future.");

            RefreshToken = token;
            RefreshTokenExpiryTime = expiry;
            UpdatedAt = DateTime.UtcNow;
        }

        public void RevokeRefreshToken()
        {
            RefreshToken = null;
            RefreshTokenExpiryTime = null;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SoftDelete()
        {
            IsDeleted = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Restore()
        {
            if (!IsDeleted) return;
            IsDeleted = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public Address AddAddress(string receiverName, string receiverPhone, string addressLine, bool isDefault = false)
        {
            if (isDefault)
                foreach (var addr in _addresses)
                    addr.ClearDefault();

            var address = new Address(receiverName, receiverPhone, addressLine, isDefault);
            _addresses.Add(address);
            return address;
        }

        public void UpdateAddress(int addressId, string receiverName, string receiverPhone, string addressLine, bool isDefault)
        {
            var address = _addresses.FirstOrDefault(a => a.Id == addressId);
            if (address == null)
                throw new DomainException("Address not found");

            if (isDefault)
                foreach (var addr in _addresses.Where(a => a.Id != addressId))
                    addr.ClearDefault();

            address.Update(receiverName, receiverPhone, addressLine, isDefault);
            UpdatedAt = DateTime.UtcNow;
        }

        public void RemoveAddress(int addressId)
        {
            var address = _addresses.FirstOrDefault(a => a.Id == addressId)
                ?? throw new DomainException("Address not found");
            _addresses.Remove(address);
            UpdatedAt = DateTime.UtcNow;
        }

        public Address GetDefaultAddress()
            => _addresses.FirstOrDefault(a => a.IsDefault);

        private static void ValidateRules(string fullName, string email, string passwordHash, UserRole role)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new DomainException("Full name is required.");

            if (string.IsNullOrWhiteSpace(email))
                throw new DomainException("Email is required.");

            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new DomainException("Password hash is required.");

            if (!Enum.IsDefined(role))
                throw new DomainException("User role is invalid.");
        }
    }
}


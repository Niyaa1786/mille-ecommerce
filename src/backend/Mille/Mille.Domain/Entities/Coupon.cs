using Mille.Domain.Exceptions;
using Mille.Domain.Enums;

namespace Mille.Domain.Entities
{
    public class Coupon
    {
        public int Id { get; private set; }
        public string Code { get; private set; } = string.Empty;
        public string? Description { get; private set; }
        public DiscountType DiscountType { get; private set; }
        public decimal DiscountValue { get; private set; }
        public decimal? MinOrderAmount { get; private set; }
        public decimal? MaxDiscountAmount { get; private set; }
        public int? Quantity { get; private set; }
        public int UsedCount { get; private set; }
        public DateTime? StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public byte[] RowVersion { get; private set; } = Array.Empty<byte>();

        private Coupon() { }

        public Coupon(
            string code,
            string? description,
            DiscountType discountType,
            decimal discountValue,
            decimal? minOrderAmount,
            decimal? maxDiscountAmount,
            int? quantity,
            DateTime? startDate,
            DateTime? endDate)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new DomainException("Coupon code is required.");

            ValidateRules(discountType, discountValue, minOrderAmount, maxDiscountAmount, quantity, startDate, endDate, usedCount: 0);

            Code = NormalizeCode(code);
            Description = description;
            DiscountType = discountType;
            DiscountValue = discountValue;
            MinOrderAmount = minOrderAmount;
            MaxDiscountAmount = maxDiscountAmount;
            Quantity = quantity;
            StartDate = startDate;
            EndDate = endDate;
            UsedCount = 0;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
        }

        public static string NormalizeCode(string code) => code.Trim().ToUpperInvariant();

        public void Update(
            string? description,
            DiscountType discountType,
            decimal discountValue,
            decimal? minOrderAmount,
            decimal? maxDiscountAmount,
            int? quantity,
            DateTime? startDate,
            DateTime? endDate,
            bool isActive)
        {
            ValidateRules(discountType, discountValue, minOrderAmount, maxDiscountAmount, quantity, startDate, endDate, UsedCount);

            Description = description;
            DiscountType = discountType;
            DiscountValue = discountValue;
            MinOrderAmount = minOrderAmount;
            MaxDiscountAmount = maxDiscountAmount;
            Quantity = quantity;
            StartDate = startDate;
            EndDate = endDate;
            IsActive = isActive;
        }

        public decimal CalculateDiscount(decimal orderAmount)
        {
            EnsureCanBeApplied(orderAmount);

            var discount = DiscountType == DiscountType.Percentage
                ? Math.Round(orderAmount * DiscountValue / 100m, 2, MidpointRounding.AwayFromZero)
                : DiscountValue;

            if (MaxDiscountAmount.HasValue)
                discount = Math.Min(discount, MaxDiscountAmount.Value);

            return Math.Min(discount, orderAmount);
        }

        public void Use()
        {
            if (Quantity.HasValue && UsedCount >= Quantity)
                throw new DomainException("Coupon has reached its usage limit.");

            UsedCount++;
        }

        public void Release()
        {
            if (UsedCount > 0)
                UsedCount--;
        }

        private void EnsureCanBeApplied(decimal orderAmount)
        {
            var now = DateTime.UtcNow;

            if (!IsActive)
                throw new DomainException("Coupon is not active.");

            if (StartDate.HasValue && now < StartDate)
                throw new DomainException("Coupon is not valid yet.");

            if (EndDate.HasValue && now > EndDate)
                throw new DomainException("Coupon has expired.");

            if (Quantity.HasValue && UsedCount >= Quantity)
                throw new DomainException("Coupon has reached its usage limit.");

            if (orderAmount <= 0)
                throw new DomainException("Order amount must be greater than zero.");

            if (MinOrderAmount.HasValue && orderAmount < MinOrderAmount)
                throw new DomainException($"Order must be at least {MinOrderAmount:0.##} to use this coupon.");
        }

        private static void ValidateRules(
            DiscountType discountType,
            decimal discountValue,
            decimal? minOrderAmount,
            decimal? maxDiscountAmount,
            int? quantity,
            DateTime? startDate,
            DateTime? endDate,
            int usedCount)
        {
            if (discountValue <= 0)
                throw new DomainException("Discount value must be greater than zero.");

            if (discountType == DiscountType.Percentage && discountValue > 100)
                throw new DomainException("Percentage discount cannot exceed 100.");

            if (minOrderAmount.HasValue && minOrderAmount < 0)
                throw new DomainException("Minimum order amount cannot be negative.");

            if (maxDiscountAmount.HasValue && maxDiscountAmount <= 0)
                throw new DomainException("Maximum discount amount must be greater than zero.");

            if (quantity.HasValue && quantity <= 0)
                throw new DomainException("Quantity must be greater than zero.");

            if (quantity.HasValue && quantity.Value < usedCount)
                throw new DomainException("Quantity cannot be less than the number of times the coupon has been used.");

            if (startDate.HasValue && endDate.HasValue && endDate <= startDate)
                throw new DomainException("End date must be after start date.");
        }
    }
}

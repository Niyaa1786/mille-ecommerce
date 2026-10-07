
namespace Mille.Domain.Entities
{
    public class CouponUsage
    {
        public int Id { get; private set; }
        public int CouponId { get; private set; }
        public Guid UserId { get; private set; }
        public Guid OrderId { get; private set; }
        public DateTime UsedAt { get; private set; }

        public Coupon Coupon { get; private set; }
        public User User { get; private set; }
        public Order Order { get; private set; }

        private CouponUsage() { }

        public CouponUsage(int couponId, Guid userId, Guid orderId)
        {
            CouponId = couponId;
            UserId = userId;
            OrderId = orderId;
            UsedAt = DateTime.UtcNow;
        }
    }
}

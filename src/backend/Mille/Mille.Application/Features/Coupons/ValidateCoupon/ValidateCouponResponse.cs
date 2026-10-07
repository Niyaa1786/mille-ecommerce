namespace Mille.Application.Features.Coupons.ValidateCoupon
{
    public class ValidateCouponResponse
    {
        public string Code { get; set; } = string.Empty;
        public string DiscountType { get; set; } = string.Empty;
        public decimal DiscountValue { get; set; }
        public decimal OrderAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalAmount { get; set; }
        public string Message { get; set; } = "Coupon is valid.";
    }
}

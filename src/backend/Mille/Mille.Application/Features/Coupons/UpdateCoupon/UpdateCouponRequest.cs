using Mille.Domain.Enums;
using System.Text.Json.Serialization;

namespace Mille.Application.Features.Coupons.UpdateCoupon
{
    public class UpdateCouponRequest
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string? Description { get; set; }
        public DiscountType DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal? MinOrderAmount { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
        public int? Quantity { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
    }
}

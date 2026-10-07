using System.Text.Json.Serialization;

namespace Mille.Application.Features.Coupons.GetCoupon
{
    public class GetCouponRequest
    {
        [JsonIgnore]
        public int Id { get; set; }
    }
}

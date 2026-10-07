using System.Text.Json.Serialization;

namespace Mille.Application.Features.Coupons.DeleteCoupon
{
    public class DeleteCouponRequest
    {
        [JsonIgnore]
        public int Id { get; set; }
    }
}

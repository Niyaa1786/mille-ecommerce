namespace Mille.Application.Features.Coupons.GetCoupons
{
    public class GetCouponsRequest
    {
        public string? Keyword { get; set; }
        public bool? IsActive { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}

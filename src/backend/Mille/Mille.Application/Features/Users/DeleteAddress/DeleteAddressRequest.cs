namespace Mille.Application.Features.Users.DeleteAddress
{
    public class DeleteAddressRequest
    {
        public Guid UserId { get; set; }
        public int AddressId { get; set; }
    }
}

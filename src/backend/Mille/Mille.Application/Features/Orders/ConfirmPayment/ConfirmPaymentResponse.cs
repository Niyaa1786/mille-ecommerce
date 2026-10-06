namespace Mille.Application.Features.Orders.ConfirmPayment
{
    public class ConfirmPaymentResponse
    {
        public string Status { get; set; } = string.Empty;
        public DateTime? PaidAt { get; set; }
        public string Message { get; set; } = "Payment confirmed successfully.";
    }
}

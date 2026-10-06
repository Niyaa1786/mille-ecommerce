using Mille.Application.Common.Exceptions;
using Mille.Application.Common.Interfaces;
using Mille.Domain.Enums;

namespace Mille.Application.Features.Orders.ConfirmPayment
{
    public class ConfirmPaymentUseCase(IUnitOfWork unitOfWork) : IUseCase<ConfirmPaymentRequest, ConfirmPaymentResponse>
    {
        public async Task<ConfirmPaymentResponse> ExecuteAsync(ConfirmPaymentRequest request, CancellationToken ct = default)
        {
            var payment = await unitOfWork.Payments.GetByOrderIdAsync(request.OrderId, ct);
            if(payment == null )
                throw new NotFoundException("Payment not found.");

            if(payment.Method != PaymentMethod.COD)
                throw new AppValidationException(nameof(payment.Method), "This endpoint only supports COD payments.");

            payment.Complete();

            await unitOfWork.SaveChangesAsync(ct);
            return new ConfirmPaymentResponse
            {
                Status = payment.Status.ToString(),
                PaidAt = payment.PaidAt,
                Message = "Payment confirmed successfully"
            };
        }
    }
}

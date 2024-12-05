using static apiWorkflowHub.DTO.Order.LinePayDTO;

namespace apiWorkflowHub.Service
{
    public interface ILinePayService
    {
        Task<PaymentResponseDto> Request(PaymentRequestDto dto);
        Task<PaymentConfirmResponseDto> Confirm(string transactionId, string orderId, PaymentConfirmDto dto);

    }
}

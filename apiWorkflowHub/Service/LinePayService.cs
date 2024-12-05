using System.Text;
using static apiWorkflowHub.DTO.Order.LinePayDTO;
using apiWorkflowHub.Common;

namespace apiWorkflowHub.Service
{
    public class LinePayService : ILinePayService
    {
        private readonly string channelId = "2006575222";
        private readonly string channelSecretKey = "1c02906ab603f523d2f71eb435f93555";
        private readonly string linePayBaseApiUrl = "https://sandbox-api-pay.line.me";
        private readonly JsonProvider _jsonProvider;
        private HttpClient client;

        public LinePayService()
        {
            client = new HttpClient();
            _jsonProvider = new JsonProvider();
        }

        public async Task<PaymentResponseDto> Request(PaymentRequestDto dto)
        {
            PaymentResponseDto linePayResponse;
            var json = _jsonProvider.Serialize(dto);
            // 產生 GUID Nonce
            var nonce = Guid.NewGuid().ToString();
            // 要放入 signature 中的 requestUrl
            var requestUrl = "/v3/payments/request";

            //使用 channelSecretKey & requestUrl & jsonBody & nonce 做簽章
            var signature = SignatureProvider.HMACSHA256(channelSecretKey, channelSecretKey + requestUrl + json + nonce);

            using (HttpClient client = new())
            {
                var request = new HttpRequestMessage(HttpMethod.Post, linePayBaseApiUrl + requestUrl)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };
                // 帶入 Headers
                client.DefaultRequestHeaders.Add("X-LINE-ChannelId", channelId);
                client.DefaultRequestHeaders.Add("X-LINE-Authorization-Nonce", nonce);
                client.DefaultRequestHeaders.Add("X-LINE-Authorization", signature);

                var response = await client.SendAsync(request);
                var resultString = await response.Content.ReadAsStringAsync();
                linePayResponse = _jsonProvider.Deserialize<PaymentResponseDto>(resultString);
                linePayResponse.Info.StringTransactionId = linePayResponse.Info.TransactionId.ToString();
            }

            return linePayResponse;
        }

        public async Task<PaymentConfirmResponseDto> Confirm(string transactionId, string orderId, PaymentConfirmDto dto)
        {
            PaymentConfirmResponseDto result;

            var json = _jsonProvider.Serialize(dto);

            using (HttpClient client = new())
            {
                var nonce = Guid.NewGuid().ToString();
                var requestUrl = string.Format("/v3/payments/{0}/confirm", transactionId);
                var signature = SignatureProvider.HMACSHA256(channelSecretKey, channelSecretKey + requestUrl + json + nonce);
                var request = new HttpRequestMessage(HttpMethod.Post, linePayBaseApiUrl + requestUrl)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };
                client.DefaultRequestHeaders.Add("X-LINE-ChannelId", channelId);
                client.DefaultRequestHeaders.Add("X-LINE-Authorization-Nonce", nonce);
                client.DefaultRequestHeaders.Add("X-LINE-Authorization", signature);

                var response = await client.SendAsync(request);
                result = _jsonProvider.Deserialize<PaymentConfirmResponseDto>(await response.Content.ReadAsStringAsync());
            }
            return result;
        }
    }
}

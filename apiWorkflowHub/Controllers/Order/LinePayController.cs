using Microsoft.AspNetCore.Mvc;
using apiWorkflowHub.ContextModels;
using System.Web.Http;
using System.Web.Http.Cors;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;
using FromBodyAttribute = Microsoft.AspNetCore.Mvc.FromBodyAttribute;
using HttpPostAttribute = Microsoft.AspNetCore.Mvc.HttpPostAttribute;
using System.Text.Json;
using apiWorkflowHub.DTO.Order;
using LineBotMessage.Dtos;
using static apiWorkflowHub.DTO.Order.LinePayDTO;
using apiWorkflowHub.Common;
using System.Text;
using Newtonsoft.Json;
using System.Security.Cryptography;
using Newtonsoft.Json.Serialization;
using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;



namespace apiWorkflowHub.Controllers.Order
{
    
    [Route("api/[controller]")]
    [ApiController]
    //[EnableCors(origins: "*", headers: "*", methods: "*")]

    public class LinePayController : ControllerBase
    {

        //linepay要接的
         private static readonly Dictionary<string, OrderRequestDto> OrderPayments = new Dictionary<string, OrderRequestDto>();
        private readonly string _channelId;
        private readonly string _channelSecret;
        private readonly string _linePayApiUrl;
        private readonly string channelId = "2006569835";  // 從 LINE 開發者平台取得
        private readonly string channelSecretKey = "296a0f6fdb8fbae0479d8a8171acc9a0";  // 從 LINE 開發者平台取得
        private readonly string linePayBaseApiUrl = "https://sandbox-api-pay.line.me";  // Sandbox API URL
        private readonly HttpClient _httpClient;
        private readonly SOPMarketContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<LinePayController> _logger;

        public LinePayController(SOPMarketContext context, HttpClient httpClient, IConfiguration configuration)
        {
            _context = context;
            _httpClient = httpClient;
            _configuration = configuration;

            // 從配置中讀取值
            _channelId = "2006569835";
            _channelSecret = "296a0f6fdb8fbae0479d8a8171acc9a0";
            _linePayApiUrl = "https://sandbox-api-pay.line.me";
        }

        // 新增 LINE Pay 請求支付端點
        [HttpPost("linepay/request")]
        public async Task<IActionResult> RequestLinePayment([FromBody] OrderRequestDto orderRequest)
        {
            try
            {
                // 檢查輸入值
                if (orderRequest == null || orderRequest.amount <= 0)
                {
                    return BadRequest(new { message = "無效的訂單金額" });
                }
                int orderAmount = orderRequest.price;

                Console.WriteLine($"Received Amount: {orderRequest.amount}");

                // 組成訂單編號
                //string currentTimeString = DateTime.Now.ToString("yyyyMMddHHmmss");
                string orderId = "USER" + DateTime.Now.ToString("yyyyMMddHHmmss");

               
                // 使用定義好的 DTO 建立請求
                var paymentRequest = new LinePayDTO.PaymentRequestDto
                {
                    Amount = orderRequest.amount,
                    Currency = "TWD",
                    OrderId = orderId,
                    Packages = new List<LinePayDTO.PackageDto>
            {
                new LinePayDTO.PackageDto
                {
                    Id = orderId,
                    Amount = orderRequest.amount,
                    Products = new List<LinePayDTO.LinePayProductDto>
                    {
                        new LinePayDTO.LinePayProductDto
                        {
                            Name = "SOP產品",
                            Quantity = 1,
                            Price = orderRequest.amount
                        }
                    }
                }
            },
                    RedirectUrls = new LinePayDTO.RedirectUrlsDto
                    {
                        ConfirmUrl = "http://localhost:4200/order/order",  // 使用完整的 HTTPS URL
                        CancelUrl = "https://localhost:7146/api/LinePay/cancel"
                    }
                };

                var requestUrl = "/v3/payments/request";
                var nonce = Guid.NewGuid().ToString();
                var jsonPayload = JsonConvert.SerializeObject(paymentRequest, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = new CamelCasePropertyNamesContractResolver() // 確保屬性名稱是小寫開頭
                });

                // 使用正確的簽章格式
                //var jsonPayload = JsonConvert.SerializeObject(requestPayload);
                var signatureData = channelSecretKey + requestUrl + jsonPayload + nonce;
                var signature = Convert.ToBase64String(
                    new HMACSHA256(Encoding.UTF8.GetBytes(channelSecretKey))
                        .ComputeHash(Encoding.UTF8.GetBytes(signatureData))
                );


                // 使用 HttpRequestMessage 來設定請求
                using var request = new HttpRequestMessage(HttpMethod.Post, $"{linePayBaseApiUrl}{requestUrl}");
                request.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");


                // 設定標頭

                request.Headers.Add("X-LINE-ChannelId", _channelId);
                request.Headers.Add("X-LINE-Authorization-Nonce", nonce);
                request.Headers.Add("X-LINE-Authorization", signature);

                // 印出請求資訊以便偵錯
                Console.WriteLine($"Request URL: {_linePayApiUrl}{requestUrl}");
                Console.WriteLine($"Request Payload: {jsonPayload}");
                Console.WriteLine($"Channel ID: {_channelId}");
                Console.WriteLine($"Nonce: {nonce}");
                Console.WriteLine($"Signature: {signature}");

                // 發送請求
                var response = await _httpClient.SendAsync(request);
                var responseData = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Response Status: {response.StatusCode}");
                Console.WriteLine($"Response Content: {responseData}");
                if (!response.IsSuccessStatusCode)
                {
                    return BadRequest(new { message = "LINE Pay API 請求失敗", error = responseData });
                }

                var linePayResponse = JsonConvert.DeserializeObject<LinePayDTO.PaymentResponseDto>(responseData);
                if (linePayResponse?.info?.paymentUrl?.web == null)
                {
                    return BadRequest(new { message = "無效的 LINE Pay 回應格式" });
                }

                return Ok(responseData);  // 直接返回 LINE Pay 的回應
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return BadRequest(new { message = "處理付款請求時發生錯誤", error = ex.Message });
            }
        }

        // 新增 LINE Pay 確認支付端點
        //[HttpPost("linepay/confirm/{transactionId}")]
        //public async Task<IActionResult> ConfirmLinePayment(string transactionId, [FromBody] PaymentConfirmDto confirmRequest)
        //{
        //    var requestBody = new
        //    {
        //        amount = confirmRequest.Amount,
        //        currency = "TWD"
        //    };

        //    _httpClient.DefaultRequestHeaders.Add("X-LINE-ChannelId", _channelId);
        //    _httpClient.DefaultRequestHeaders.Add("X-LINE-ChannelSecret", _channelSecret);

        //    var response = await _httpClient.PostAsJsonAsync(
        //        $"{_linePayApiUrl}/confirm/{transactionId}",
        //        requestBody);

        //    var content = await response.Content.ReadAsStringAsync();
        //    return Ok(content);
        //}

        private string GenerateSignature(string jsonBody, string secret)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA256(System.Text.Encoding.UTF8.GetBytes(secret)))
            {
                var signature = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(jsonBody));
                return Convert.ToBase64String(signature);
            }
        }

        [HttpGet("confirm")]
        public async Task<IActionResult> ConfirmPayment([FromQuery] string transactionId, [FromQuery] string orderId)
        {
            try
            {
                var nonce = Guid.NewGuid().ToString();
                var apiUrl = $"/v3/payments/{transactionId}/confirm";

                // 從字典中獲取訂單資訊
                if (!OrderPayments.TryGetValue(orderId, out var orderRequest))
                {
                    return BadRequest("找不到訂單資訊");
                }

                // 準備確認請求
                var confirmRequest = new
                {
                    amount = orderRequest.amount,
                    currency = "TWD"
                };

                var jsonPayload = JsonConvert.SerializeObject(confirmRequest);
                var signatureString = _channelSecret + apiUrl + jsonPayload + nonce;
                var signature = GenerateSignature(jsonPayload, _channelSecret);

                // 建立 HTTP 請求
                var request = new HttpRequestMessage(HttpMethod.Post, _linePayApiUrl + apiUrl)
                {
                    Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json")
                };

                // 添加 LINE Pay 所需的標頭
                request.Headers.Add("X-LINE-ChannelId", _channelId);
                request.Headers.Add("X-LINE-Authorization-Nonce", nonce);
                request.Headers.Add("X-LINE-Authorization", signature);

                // 發送請求到 LINE Pay
                var response = await _httpClient.SendAsync(request);
                var responseData = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    // 清理暫存的訂單資訊
                    OrderPayments.Remove(orderId);

                    // 重定向到訂單頁面
                    return Redirect("http://localhost:4200/order/order");
                }
                else
                {
                    _logger.LogError($"LINE Pay 確認失敗: {responseData}");
                    return Redirect("http://localhost:4200/error");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"處理付款確認時發生錯誤: {ex.Message}");
                return Redirect("http://localhost:4200/error");
            }
        }

    }

    public class OrderRequestDto
    {
        public int price { get; set; }
        public int? couponId { get; set; }
        public string address { get; set; }
        public int amount { get; set; }
    }
    //public class InfoData
    //{
    //    public PaymentUrlData paymentUrl { get; set; }
    //    public string transactionId { get; set; }
    //}

    //public class PaymentConfirmDto
    //{
    //    public decimal Amount { get; set; }
    //}
}


using Newtonsoft.Json;

namespace apiWorkflowHub.DTO.Order
{
    public class RecaptchaResponseDTO
    {

        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("score")]
        public float Score { get; set; }

        [JsonProperty("error-codes")]
        public IEnumerable<string> ErrorCodes { get; set; }
    }
}

using System.Text.Json.Serialization;

namespace apiWorkflowHub.DTO.Workflow
{
    public class TMembSopListDTO
    {
        [JsonPropertyName("FSopid")]// 要送到前端的，所以Json要維持跟後端一樣的大寫
        public int FSopid { get; set; }

        [JsonPropertyName("FMemberId")]
        public int? FMemberId { get; set; }

        [JsonPropertyName("FSopType")]
        public byte? FSopType { get; set; }

        [JsonPropertyName("FSopName")]
        public string FSopName { get; set; }

        [JsonPropertyName("FSopFlowImagePath")]
        public string FSopFlowImagePath { get; set; }

        [JsonPropertyName("FJobItemId")]
        public int? FJobItemId { get; set; }

        [JsonPropertyName("JobItem")]
        public string JobItem { get; set; }

        [JsonPropertyName("FIndustryId")]
        public int? FIndustryId { get; set; }

        [JsonPropertyName("Industry")]
        public string Industry { get; set; }

        [JsonPropertyName("FFileStatus")]
        public string FFileStatus { get; set; }

        [JsonPropertyName("FEditTime")]
        public string FEditTime { get; set; }

        [JsonPropertyName("FSharePermission")]
        public string FSharePermission { get; set; }
    }
}

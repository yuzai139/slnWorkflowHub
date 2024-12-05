using System.Text.Json.Serialization;

namespace apiWorkflowHub.DTO.Workflow
{
    public class TPuberSopListDTO
    {
        [JsonPropertyName("FSopid")]// 要送到前端的，所以Json要維持跟後端一樣的大寫
        public int FSopid { get; set; }

        [JsonPropertyName("FMemberId")]
        public int? FMemberId { get; set; }

        [JsonPropertyName("FPublisherId")]
        public int? FPublisherId { get; set; }

        [JsonPropertyName("FSopType")]
        public byte? FSopType { get; set; }

        [JsonPropertyName("FSopName")]
        public string FSopName { get; set; }

        [JsonPropertyName("FPubSopImagePath")]
        public string FPubSopImagePath { get; set; }

        [JsonPropertyName("FJobItemId")]
        public int? FJobItemId { get; set; }

        [JsonPropertyName("JobItem")]
        public string JobItem { get; set; }

        [JsonPropertyName("FIndustryId")]
        public int? FIndustryId { get; set; }

        [JsonPropertyName("Industry")]
        public string Industry { get; set; }

        [JsonPropertyName("FReleaseStatus")]
        public string FReleaseStatus { get; set; }

        [JsonPropertyName("FReleaseTime")]
        public string FReleaseTime { get; set; }

    }
}

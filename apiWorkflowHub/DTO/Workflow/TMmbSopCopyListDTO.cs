using System.Text.Json.Serialization;

namespace apiWorkflowHub.DTO.Workflow
{
    public class TMmbSopCopyListDTO
    {
        [JsonPropertyName("FSopid")]
        public int FSopid { get; set; }

        [JsonPropertyName("FMemberId")]
        public int? FMemberId { get; set; }

        [JsonPropertyName("FSopType")]
        public byte? FSopType { get; set; }

        [JsonPropertyName("FSopName")]
        public string FSopName { get; set; }

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
    }
}

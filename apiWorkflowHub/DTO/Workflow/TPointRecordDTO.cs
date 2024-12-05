using System.Text.Json.Serialization;

namespace apiWorkflowHub.DTO.Workflow
{
    public class TPointRecordDTO
    {
        [JsonPropertyName("FPointRecordId")]
        public int FPointRecordId { get; set; }

        [JsonPropertyName("FMemberId")]
        public int? FMemberId { get; set; }

        [JsonPropertyName("FPointRecord")]
        public string FPointRecord { get; set; }

        [JsonPropertyName("FExplanation")]
        public string FExplanation { get; set; }

        [JsonPropertyName("FRecordTime")]
        public string FRecordTime { get; set; }
    }
}

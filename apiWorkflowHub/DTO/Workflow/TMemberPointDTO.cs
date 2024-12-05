using System.Text.Json.Serialization;

namespace apiWorkflowHub.DTO.Workflow
{
    public class TMemberPointDTO
    {
        [JsonPropertyName("FMemberId")]
        public int FMemberId { get; set; }

        [JsonPropertyName("FName")]
        public string FName { get; set; }

        [JsonPropertyName("FMemberPoints")]
        public int? FMemberPoints { get; set; }

        [JsonPropertyName("FMemberShip")]
        public bool? FMemberShip { get; set; }

    }
}

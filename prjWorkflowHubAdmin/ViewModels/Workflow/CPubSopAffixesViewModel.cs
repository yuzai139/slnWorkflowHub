using System.Text.Json.Serialization;

namespace prjWorkflowHubAdmin.ViewModels.Workflow
{
    public class CPubSopAffixesViewModel
    {
        [JsonPropertyName("FSopaffixId")]
        public int FSopaffixId { get; set; }

        [JsonPropertyName("FSopid")]
        public int? FSopid { get; set; }

        [JsonPropertyName("FAffixName")]
        public string? FAffixName { get; set; }

        [JsonPropertyName("FAffixPath")]
        public string? FAffixPath { get; set; }
    }
}

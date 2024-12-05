using System.Text.Json.Serialization;

namespace apiWorkflowHub.DTO.Workflow
{
    public class TSopDTO
    {
        [JsonPropertyName("FSopid")]
        public int FSopid { get; set; }

        [JsonPropertyName("FMemberId")]
        public int? FMemberId { get; set; }

        [JsonPropertyName("FPublisherId")]
        public int? FPublisherId { get; set; }

        [JsonPropertyName("FSopType")]
        public byte? FSopType { get; set; }

        [JsonPropertyName("FSopName")]
        public string? FSopName { get; set; }

        [JsonPropertyName("FSopDescription")]
        public string? FSopDescription { get; set; }

        [JsonPropertyName("FPubContent")]
        public string? FPubContent { get; set; }

        [JsonPropertyName("FSopFlowImagePath")]
        public string? FSopFlowImagePath { get; set; }

        [JsonPropertyName("FPubSopImagePath")]
        public string? FPubSopImagePath { get; set; }

        [JsonPropertyName("FJobItemId")]
        public int? FJobItemId { get; set; }

        [JsonPropertyName("JobItem")]
        public string? JobItem { get; set; } // JobItem資料表的值

        [JsonPropertyName("FIndustryId")]
        public int? FIndustryId { get; set; }

        [JsonPropertyName("Industry")]
        public string? Industry { get; set; } // Industry資料表的值

        [JsonPropertyName("FBusiness")]
        public string? FBusiness { get; set; }

        [JsonPropertyName("FCustomer")]
        public string? FCustomer { get; set; }

        [JsonPropertyName("FCompanySize")]
        public string? FCompanySize { get; set; }

        [JsonPropertyName("FDepartment")]
        public string? FDepartment { get; set; }

        [JsonPropertyName("FShareUrl")]
        public string? FShareUrl { get; set; }

        [JsonPropertyName("FEditTime")]
        public string? FEditTime { get; set; }

        [JsonPropertyName("FSharePermission")]
        public string? FSharePermission { get; set; }

        [JsonPropertyName("FFileStatus")]
        public string? FFileStatus { get; set; }

        [JsonPropertyName("FReleaseTime")]
        public string? FReleaseTime { get; set; }

        [JsonPropertyName("FReleaseStatus")]
        public string? FReleaseStatus { get; set; }

        [JsonPropertyName("FProductUrl")]
        public string? FProductUrl { get; set; }

        [JsonPropertyName("FIsRelease")]
        public bool? FIsRelease { get; set; } // 是否曾經發佈過

        [JsonPropertyName("FPrice")]
        public decimal? FPrice { get; set; }

        [JsonPropertyName("FSalePoints")]
        public decimal? FSalePoints { get; set; }
    }
}

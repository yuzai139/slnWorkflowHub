﻿using System.Text.Json.Serialization;

namespace apiWorkflowHub.DTO.Workflow
{
    public class TMemberSopDTO
    {
        [JsonPropertyName("FSopid")]
        public int FSopid { get; set; }

        [JsonPropertyName("FMemberId")]
        public int? FMemberId { get; set; }

        [JsonPropertyName("FSopType")]
        public byte? FSopType { get; set; }

        [JsonPropertyName("FSopName")]
        public string? FSopName { get; set; }

        [JsonPropertyName("FSopDescription")]
        public string? FSopDescription { get; set; }

        [JsonPropertyName("FSopFlowImagePath")]
        public string? FSopFlowImagePath { get; set; }

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
    }
}
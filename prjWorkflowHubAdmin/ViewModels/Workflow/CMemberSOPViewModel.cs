namespace prjWorkflowHubAdmin.ViewModels.Workflow
{
    public class CMemberSopViewModel
    {
        public int FSopid { get; set; }

        public int? FMemberId { get; set; }

        public byte? FSopType { get; set; }

        public string? FSopName { get; set; }

        public string? FSopDescription { get; set; }

        public string? FSopFlowImagePath { get; set; }

        public int? FJobItemId { get; set; }

        public string JobItem {  get; set; } //邏輯資料

        public int? FIndustryId { get; set; }

        public string Industry {  get; set; } //邏輯資料

        public string? FBusiness { get; set; }

        public string? FCustomer { get; set; }

        public string? FCompanySize { get; set; }

        public string? FDepartment { get; set; }

        public string? FShareUrl { get; set; }

        public string? FEditTime { get; set; }

        public string? FSharePermission { get; set; }

        public string? FFileStatus { get; set; }

    }
}

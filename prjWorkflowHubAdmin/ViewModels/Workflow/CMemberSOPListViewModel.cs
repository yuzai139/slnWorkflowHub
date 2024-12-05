namespace prjWorkflowHubAdmin.ViewModels.Workflow
{
    public class CMemberSOPListViewModel
    {
        public int FSopid { get; set; }

        public int? FMemberId { get; set; }
        public string MemberName { get; set; }//Member資料表的值

        public byte? FSopType { get; set; }

        public string FSopName { get; set; }

        public string FSopFlowImagePath { get; set; }

        public int? FJobItemId { get; set; }

        public int? FIndustryId { get; set; }

        public string FFileStatus { get; set; }
    }
}

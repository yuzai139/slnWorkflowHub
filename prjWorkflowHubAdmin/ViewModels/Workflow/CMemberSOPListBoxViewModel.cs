namespace prjWorkflowHubAdmin.ViewModels.Workflow
{
    public class CMemberSOPListBoxViewModel
    {
        public string MemberName { get; set; }  // 會員名稱
        public int? MemberId { get; set; } // 會員Id
        public List<CMemberSOPListViewModel> MemberSOPList { get; set; }  // SOP 資料列表                                             
    }
}

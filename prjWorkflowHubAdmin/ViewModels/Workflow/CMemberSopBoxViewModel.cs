namespace prjWorkflowHubAdmin.ViewModels.Workflow
{
    public class CMemberSopBoxViewModel
    {
        public string MemberName { get; set; }  // 會員名稱
        public CMemberSopViewModel MemberSopViewModel { get; set; }  // SOP 資料列表

        public IFormFile? DiagramPng { get; set; }// 新增這個屬性來處理圖片上傳
    }
}

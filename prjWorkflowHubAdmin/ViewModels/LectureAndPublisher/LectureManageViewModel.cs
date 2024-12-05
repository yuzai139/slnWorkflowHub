using System.ComponentModel;

namespace prjWorkflowHubAdmin.ViewModels.LectureAndPublisher
{
    public class LectureManageViewModel
    {
        public int FLecRecordId { get; set; }
        [DisplayName("講座名稱")]
        public string FLecName { get; set; }
        public int FLectureId { get; set; }
        public int FMemberId { get; set; }
        [DisplayName("會員姓名")]
        public string FName { get; set; }
        [DisplayName("會員電話")]
        public string FPhone { get; set; }
        [DisplayName("會員Email")]
        public string FEmail { get; set; }
    }
}

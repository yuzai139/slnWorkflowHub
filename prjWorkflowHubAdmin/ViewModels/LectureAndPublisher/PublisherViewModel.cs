using System.ComponentModel;

namespace prjWorkflowHubAdmin.ViewModels.LectureAndPublisher
{
    public class PublisherViewModel
    {
        [DisplayName("發佈者ID")]
        public int FPublisherId { get; set; }
        [DisplayName("會員ID")]
        public int FMemberId { get; set; }
        [DisplayName("發佈者名稱")]
        public string FPubName { get; set; }
        [DisplayName("會員名稱")]
        public string fMemberName { get; set; }
        [DisplayName("發佈者簡介")]
        public string FPubDescription { get; set; }
        [DisplayName("發佈者名照片")]
        public byte[] FPubImage { get; set; }
        public string? FLecImagePath { get; set; }
    }
}

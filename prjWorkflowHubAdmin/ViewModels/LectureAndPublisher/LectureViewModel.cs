using System.ComponentModel;

namespace prjWorkflowHubAdmin.ViewModels.LectureAndPublisher
{
    public class LectureViewModel //?會影響是否必填
    {        
        public int FLectureId { get; set; }
        [DisplayName("講座名稱")]
        public string FLecName { get; set; }
        [DisplayName("發佈者名稱")]
        public string FPubName { get; set; }
        [DisplayName("發佈者ID")]
        public int FPublisherId { get; set; }

        [DisplayName("講座價錢")]
        public decimal? FLecPrice { get ;  set; }//{ return Math.Round((decimal) FLecPrice, 0)
        [DisplayName("講座點數")]
        public decimal? FLecPoints { get; set; }// { return Math.Round((decimal)FLecPoints, 0); }
        [DisplayName("講座內容")]
        public string FLecDescription { get; set; }
        [DisplayName("講座形式")]
        public bool FOnline { get; set; }
        [DisplayName("講座日期")]
        public string? FLecDate { get; set; }
        [DisplayName("講座地點")]
        public string? FLecLocation { get; set; }
        [DisplayName("講座限制人數")]
        public int? FLecLimit { get; set; }
        [DisplayName("講座連結")]
        public string? FLink { get; set; }
        public byte[]? FLecImage { get; set; }
        public string? FLecImagePath { get; set; }

    }
}

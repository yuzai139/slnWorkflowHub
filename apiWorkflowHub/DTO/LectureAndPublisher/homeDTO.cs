namespace apiWorkflowHub.DTO.LectureAndPublisher
{
    public class homeDTO
    {
        //Id不解釋
        public int fSOPID { get; set; }
        public string fSopName { get; set; }
        //圖片路徑，傳URL
        public string fPubSopImagePath { get; set; }

        public decimal? fPrice { get; set; }
        public decimal? fSalePoints { get; set; }
        //發布狀態 抓已發布==2
        public byte? FSopType { get; set; }
        //以下篩選用
        public int? fJobItemId { get; set; }
        public int? fIndustryId { get; set; }
        public string fCompanySize { get; set; }
        public string fReleaseTime { get; set; }
        //本地圖片路徑
        public string fPubSopImageUrl { get; set; }
        //傳送過去的base64
        public string fPassImage { get; set; }
    }
}

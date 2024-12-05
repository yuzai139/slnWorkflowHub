namespace apiWorkflowHub.DTO.LectureAndPublisher
{
    public class workFlowProdDTO
    {
        public int fSOPID { get; set; }
        public int fPubId { get; set; }
        public string fSopName { get; set; }
        public string fPubName { get; set; }
        public string FPubContent { get; set; }
        //圖片路徑，傳URL
        public string fPubSopImagePath { get; set; }

        public decimal? fPrice { get; set; }
        public decimal? fSalePoints { get; set; }
        //發布狀態 抓已發布就好 (未發布、發布中)
        public string fReleaseStatus { get; set; }
        //以下篩選用
        public int? fJobItemId { get; set; }
        public int? fIndustryId { get; set; }
        public string fCompanySize { get; set; }
        public string fReleaseTime { get; set; }
    }
}

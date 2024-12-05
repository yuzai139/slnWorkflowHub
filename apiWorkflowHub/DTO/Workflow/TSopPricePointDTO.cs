namespace apiWorkflowHub.DTO.Workflow
{
    public class TSopPricePointDTO
    {
        public int FSopid { get; set; }

        public int? FMemberId { get; set; }

        public int? FPublisherId { get; set; }
        public byte? FSopType { get; set; }

        public string FSopName { get; set; }
        public decimal? FPrice { get; set; }
        public decimal? FSalePoints { get; set; }
    }
}

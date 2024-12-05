namespace apiWorkflowHub.DTO.LectureAndPublisher
{
    public class forCreatePublisher
    {
        public int fMemberId { get; set; }

        public string fPubName { get; set; }

        public string? fPubDescription { get; set; }
        public string? fPubLink { get; set; }

        public IFormFile? fPubImage { get; set; }
        
        //public string? fPubImagePath { get; set; }

        public string? fPubStatus { get; set; }

        //public string? fPubCreateTime { get; set; }

        //public int? fPublisherId { get; set; }
    }
}

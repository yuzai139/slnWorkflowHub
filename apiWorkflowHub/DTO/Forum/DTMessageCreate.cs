using System.ComponentModel.DataAnnotations;

namespace apiWorkflowHub.DTO.Forum
{
    public class DTMessageCreate
    {
        [Required]
        public int FArticleId { get; set; }

        [Required]
        public int FMemberId { get; set; }

        [Required]
        [StringLength(5000)]
        public string FMessageContent { get; set; }
    }
}
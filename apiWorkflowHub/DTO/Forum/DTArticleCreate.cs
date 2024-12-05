using System.ComponentModel.DataAnnotations;

namespace apiWorkflowHub.DTO.Forum
{
    public class DTArticleCreate
    {
        [Required]
        [StringLength(50)]
        public string FArticleName { get; set; }

        [Required]
        public string FArticleContent { get; set; }

        [Required]
        public int FCategoryNumber { get; set; }

        [Required]
        public int FMemberID { get; set; }
    }
}
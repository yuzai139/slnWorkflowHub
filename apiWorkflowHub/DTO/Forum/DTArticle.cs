using apiWorkflowHub.ContextModels;
using apiWorkflowHub.Controllers.DTO.Forum;
using System.ComponentModel.DataAnnotations;

namespace apiWorkflowHub.DTO.Forum
{
   public class DTArticle
{
    // 移除 Required 標記，因為新增時不需要 ID
    public int FArticleID { get; set; }
    
    [Required]
    [StringLength(50)]
    public string? FArticleName { get; set; }
    
    [Required]
    public string? FArticleContent { get; set; }
    
    public int FCategoryNumber { get; set; }
    public int FMemberID { get; set; }
    public DateTime FCreatedAt { get; set; }
    public DateTime FUpdatedAt { get; set; }

    public virtual DTCategory? FCategoryNumberNavigation { get; set; }

    // 修改 FromEntity 方法
    public static DTArticle FromEntity(TArticle article)
    {
        if (article == null) return null;
        
        return new DTArticle
        {
            FArticleID = article.FArticleId,
            FArticleName = article.FArticleName,
            FArticleContent = article.FArticleContent,
            FCategoryNumber = article.FCategoryNumber,
            FMemberID = article.FMemberId,
            FCreatedAt = article.FCreatedAt,
            FUpdatedAt = article.FUpdatedAt,
            FCategoryNumberNavigation = article.FCategoryNumberNavigation != null
                ? DTCategory.FromEntity(article.FCategoryNumberNavigation)
                :  null
        };
    }
}
}

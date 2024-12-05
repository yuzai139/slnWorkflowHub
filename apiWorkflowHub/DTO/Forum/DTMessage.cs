using apiWorkflowHub.ContextModels;
using apiWorkflowHub.DTO.Forum;
using System.ComponentModel.DataAnnotations;

public class DTMessage
{
    public int FMessageId { get; set; }

    [Required]
    public int FArticleId { get; set; }

    [Required]
    public int FMemberId { get; set; }

    [Required]
    [StringLength(500)]
    public string FMessageContent { get; set; }

    public string? FCreatedAt { get; set; }

    public string? FUpdatedAt { get; set; }

    public virtual DTArticle? FArticle { get; set; }
    
    // 加入會員資訊
    public virtual DTMember? FMember { get; set; }

    public static DTMessage FromEntity(TMessage message)
    {
        if (message == null) return null;

        return new DTMessage
        {
            FMessageId = message.FMessageId,
            FArticleId = message.FArticleId,
            FMemberId = message.FMemberId,
            FMessageContent = message.FMessageContent,
            FCreatedAt = message.FCreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
            FUpdatedAt = message.FUpdatedAt?.ToString("yyyy-MM-dd HH:mm:ss"),
            FArticle = message.FArticle != null ? DTArticle.FromEntity(message.FArticle) : null,
            FMember = message.FMember != null ? DTMember.FromEntity(message.FMember) : null
        };
    }
}
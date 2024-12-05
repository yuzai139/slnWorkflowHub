using apiWorkflowHub.ContextModels;
using System.ComponentModel.DataAnnotations;

namespace apiWorkflowHub.Controllers.DTO.Forum
{
    public class DTCategory
    {
        [Required] // 標記為必填
        public int FCategoryNumber { get; set; }

        [Required(ErrorMessage = "Category name is required.")]
        [StringLength(100)] // 限制長度
        public string? FCategoryName { get; set; }

        // 靜態方法來將 TCategory 轉換為 DTCategory
        public static DTCategory FromEntity(TCategory category)
        {
            return new DTCategory
            {
                FCategoryNumber = category.FCategoryNumber,
                FCategoryName = category.FCategoryName
            };
        }
    }
}

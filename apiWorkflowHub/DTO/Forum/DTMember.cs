using apiWorkflowHub.ContextModels;
using System.ComponentModel.DataAnnotations;

namespace apiWorkflowHub.DTO.Forum
{
    public class DTMember
    {
        [Required]
        public int FMemberId { get; set; }

        [Required]
        [StringLength(50)]
        public string FName { get; set; }

        public static DTMember FromEntity(TMember member)
        {
            if (member == null)
                return null;

            return new DTMember
            {
                FMemberId = member.FMemberId,
                FName = member.FName

            };
        }
    }
}
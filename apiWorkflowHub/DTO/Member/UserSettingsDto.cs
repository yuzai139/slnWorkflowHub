namespace apiWorkflowHub.DTO.Member
{
    public class UserSettingsDto
    {
        public int fMemberID { get; set; }
        public string fName { get; set; }
        public string fBirthday { get; set; }
        public string fPhone { get; set; }
        public string fEmail { get; set; }
        public string fPassword { get; set; }
        public string fAddress { get; set; }
        public string fPermissions { get; set; }
        public int? fMemberPoints { get; set; }

        public bool? fMemberShip { get; set; }

        public bool? fMailVerify { get; set; }

        public int? fSopexp { get; set; }
    }
}

namespace SchoolInformationSystem.Client.DTOs
{
    public class UpdateStudentDto
    {
        public int? UserId { get; set; }
        public int? NewSchoolNum { get; set; }
        public string? Email { get; set; }
    }
}

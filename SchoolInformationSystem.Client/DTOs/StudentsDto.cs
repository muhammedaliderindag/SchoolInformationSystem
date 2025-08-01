namespace SchoolInformationSystem.Client.DTOs
{
    public class StudentsDto
    {
        public int? UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }   
        public int? SchoolNumber { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
    }
}

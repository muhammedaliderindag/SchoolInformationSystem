namespace SchoolInformationSystem.Application.DTOs
{
    public class LessonList
    {
        public int LessonId { get; set; }
        public string LessonName { get; set; } = string.Empty;
        public int Credit { get; set; }
        public int Akts { get; set; }
        public int TeacherId { get; set; }
        public string TeacherName { get; set; } = string.Empty;
        public int ClassId { get; set; }
        public int Added { get; set; } = 0;
    }
}

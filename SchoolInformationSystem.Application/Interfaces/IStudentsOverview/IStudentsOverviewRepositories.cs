using SchoolInformationSystem.Application.DTOs;

namespace SchoolInformationSystem.Application.Interfaces.IStudentsOverview
{
    public interface IStudentsOverviewRepositories
    {
        Task<List<StudentsDto>> GetStudentsListAsync();
        Task<StudentsDto> GetStudentFromIdAsyncRepo(int id);
        Task<UpdateStudentDto> StudentEditPageUpdateRepo(int UserId, int NewSchoolNum, string Email);
    }
}
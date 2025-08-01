using SchoolInformationSystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolInformationSystem.Application.Interfaces
{
    public interface IStudentsOverviewService
    {
        Task<List<StudentsDto>> GetStudentsAsync();
        Task<StudentsDto> GetStudentFromIdAsync(int id);
        Task<ServiceResponse<UpdateStudentDto>> StudentEditPageUpdateService(int UserId, int NewSchoolNum, string Email);
    }
}

using SchoolInformationSystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolInformationSystem.Application.Interfaces.IStudent
{
    public interface IStudentRepositories
    {
        Task<List<LessonList>> GetLessonsAsyncRepo(int UserId);
        Task<ServiceResponse<string>> SaveSelectedLessonsRepo(List<LessonList> list,int UserId);
    }
}

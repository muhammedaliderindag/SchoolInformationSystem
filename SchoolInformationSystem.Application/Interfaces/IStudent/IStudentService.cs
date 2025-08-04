using SchoolInformationSystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolInformationSystem.Application.Interfaces.IStudent
{
    public interface IStudentService
    {
        Task<List<LessonList>> GetLessonsAsync();
    }
}

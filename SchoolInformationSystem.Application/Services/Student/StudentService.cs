using SchoolInformationSystem.Application.DTOs;
using SchoolInformationSystem.Application.Interfaces.IStudent;
using SchoolInformationSystem.Application.Interfaces.IStudentsOverview;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolInformationSystem.Application.Services.Student
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepositories _StudentRepositories;


        public StudentService(IStudentRepositories StudentRepositories)
        {
            _StudentRepositories = StudentRepositories;
        }

        public async Task<List<LessonList>> GetLessonsAsync()
        {
            
            var lessons = await _StudentRepositories.GetLessonsAsyncRepo();

            if (lessons == null)
            {
                return null; 
            }
            return lessons;
        }
    }
}

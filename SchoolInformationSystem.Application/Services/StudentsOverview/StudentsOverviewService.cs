using Microsoft.Extensions.Configuration;
using SchoolInformationSystem.Application.DTOs;
using SchoolInformationSystem.Application.Interfaces.IStudentsOverview;
using SchoolInformationSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolInformationSystem.Application.Services.StudentsOverview
{
    public class StudentsOverviewService : IStudentsOverviewService
    {
        private readonly IStudentsOverviewRepositories _StudentsOverviewRepositories;
        

        public StudentsOverviewService(IStudentsOverviewRepositories StudentsOverviewRepositories)
        {
            _StudentsOverviewRepositories = StudentsOverviewRepositories;
        }

        public async Task<StudentsDto> GetStudentFromIdAsync(int id)
        {
            var student = await _StudentsOverviewRepositories.GetStudentFromIdAsyncRepo(id);
            if (student == null)
            {
                return null; 
            }
            return student;
        }

        public async Task<List<StudentsDto>> GetStudentsAsync()
        {
            var students = await _StudentsOverviewRepositories.GetStudentsListAsync();

            if (students == null)
            {
                return null;
            }
            return students;
        }

        public async Task<ServiceResponse<UpdateStudentDto>> StudentEditPageUpdateService(int UserId, int NewSchoolNum, string Email)
        {
            var student = await _StudentsOverviewRepositories.StudentEditPageUpdateRepo(UserId, NewSchoolNum, Email);
            if (student == null)
            {
                return ServiceResponse<UpdateStudentDto>.Fail("Student not found or update failed.");
            }
            return ServiceResponse<UpdateStudentDto>.Success(student);
        }
    }
}

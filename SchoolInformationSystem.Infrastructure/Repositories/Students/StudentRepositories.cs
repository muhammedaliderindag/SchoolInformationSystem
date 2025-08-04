using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SchoolInformationSystem.Application.DTOs;
using SchoolInformationSystem.Application.Interfaces.IStudent;
using SchoolInformationSystem.Domain.Entities;
using SchoolInformationSystem.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolInformationSystem.Infrastructure.Repositories.Students
{

    public class StudentRepositories : IStudentRepositories
    {
        private readonly SchoolInformationSystemDbContext _context;

        public StudentRepositories(SchoolInformationSystemDbContext context)
        {
            _context = context;
        }

        public async Task<List<LessonList>> GetLessonsAsyncRepo()
        {
            var lessonList = await _context.Lessons
                .Include(lesson => lesson.Teacher)
                    .ThenInclude(teacher => teacher.User) 
                .AsNoTracking()
                .Select(lesson => new LessonList
                {
                    LessonId = lesson.LessonId,
                    LessonName = lesson.LessonName,
                   
                    TeacherName = lesson.Teacher.User.FirstName + " " + lesson.Teacher.User.LastName,
                    Credit = lesson.Credit ?? 0,
                    Akts = lesson.Akts ?? 0,
                    TeacherId = lesson.Teacher.TeacherId,
                    ClassId = lesson.Derslik ?? 0 
                })
                .ToListAsync(); 

            return lessonList; 
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
        }

    }
}

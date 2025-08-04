using Microsoft.EntityFrameworkCore;
using SchoolInformationSystem.Application.DTOs;
using SchoolInformationSystem.Application.Interfaces.IStudentsOverview;
using SchoolInformationSystem.Domain.Entities;
using SchoolInformationSystem.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace SchoolInformationSystem.Infrastructure.Repositories.StudentsOverview
{
    public class StudentsOverviewRepositories : IStudentsOverviewRepositories
    {
        private readonly SchoolInformationSystemDbContext _context;

        public StudentsOverviewRepositories(SchoolInformationSystemDbContext context)
        {
            _context = context;
        }

        public async Task<StudentsDto> GetStudentFromIdAsyncRepo(int id)
        {

            var userEntity = await _context.Users
                                            .Include(user => user.Student)
                                            .AsNoTracking()
                                            .FirstOrDefaultAsync(u => u.UserId == id);

            if (userEntity == null)
            {
                return null;
            }

            StudentsDto studentDto = new StudentsDto
            {
                UserId = userEntity.UserId,
                FirstName = userEntity.FirstName, 
                LastName = userEntity.LastName,
                Email = userEntity.Email,
                SchoolNumber = userEntity.Student?.SchoolNumber ?? 0,
            };

            return studentDto;
}

        public async Task<List<StudentsDto>> GetStudentsListAsync()
        {
    var studentsQuery = _context.Users
       
        .Where(user => user.Student != null) 
        .AsNoTracking() 
        .Select(user => new StudentsDto
        {
            UserId = user.UserId,
            FirstName = user.FirstName, 
            LastName = user.LastName,
            Email = user.Email,
            SchoolNumber = user.Student.SchoolNumber 
        });

    return await studentsQuery.ToListAsync();
        }

        public async Task<UpdateStudentDto> StudentEditPageUpdateRepo(int UserId, int NewSchoolNum, string Email)
        {
            var userToUpdate = await _context.Users
                                 .Where(user => user.Student != null)
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync(u => u.UserId == UserId);

            if (userToUpdate == null || userToUpdate.Student == null)
            {
                return null;
            }

            userToUpdate.Email = Email;                           
            userToUpdate.Student.SchoolNumber = NewSchoolNum;     

            await _context.SaveChangesAsync();

           
            return new UpdateStudentDto
            {
                UserId = userToUpdate.UserId,
                Email = userToUpdate.Email,
                NewSchoolNum = userToUpdate.Student.SchoolNumber
            };
        }
    }
}

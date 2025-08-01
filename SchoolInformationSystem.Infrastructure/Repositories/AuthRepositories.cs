using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SchoolInformationSystem.Application.Interfaces;
using SchoolInformationSystem.Domain.Entities;
using SchoolInformationSystem.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolInformationSystem.Infrastructure.Repositories
{

    public class AuthRepositories : IAuthRepositories
    {
        private readonly SchoolInformationSystemDbContext _context;

        public AuthRepositories(SchoolInformationSystemDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
        }

        public async Task<RefreshToken?> GetRefreshTokenByTokenAsync(string token)
        {
            // Kullanıcı bilgisini de dahil ederek token'ı getiriyoruz (Include).
            return await _context.RefreshTokens
                .Include(rt => rt.User)
                .SingleOrDefaultAsync(rt => rt.Token == token);
        }

        public async Task AddRefreshTokenAsync(RefreshToken refreshToken)
        {
            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRefreshTokenAsync(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Update(refreshToken);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UserExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task AddUserAsync(User user, Student student)
        {
            await _context.Users.AddAsync(user);
            await _context.Students.AddAsync(student);
            await _context.SaveChangesAsync();
        }
    }
}

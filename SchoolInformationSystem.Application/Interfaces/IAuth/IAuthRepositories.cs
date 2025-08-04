using SchoolInformationSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolInformationSystem.Application.Interfaces.IAuth
{
    public interface IAuthRepositories
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task<RefreshToken?> GetRefreshTokenByTokenAsync(string token);
        Task AddRefreshTokenAsync(RefreshToken refreshToken);
        Task UpdateRefreshTokenAsync(RefreshToken refreshToken); // Revoke ve rotasyon için

        Task<bool> UserExistsAsync(string email);
        Task AddUserAsync(User user, Student student);
    }
}

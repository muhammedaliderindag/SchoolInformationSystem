using SchoolInformationSystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolInformationSystem.Application.Interfaces
{
    public interface IAuthService
    {
        Task<(string? accessToken, string? refreshToken)> LoginAsync(string email, string password, string ipAddress);
        Task<(string? accessToken, string? refreshToken)> LoginAsync(string email, string ipAddress);
        Task<(string? accessToken, string? newRefreshToken)> RefreshTokenAsync(string token, string ipAddress);
        Task<bool> RevokeTokenAsync(string token, string ipAddress);
        Task<(bool Success, IEnumerable<string> Errors)> RegisterAsync(string firstName, string lastName, string email, string password);
    }
}

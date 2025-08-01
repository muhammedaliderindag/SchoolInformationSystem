using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SchoolInformationSystem.Application.DTOs;
using SchoolInformationSystem.Application.Interfaces;
using SchoolInformationSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SchoolInformationSystem.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepositories _authRepositories; 
        private readonly IConfiguration _configuration;

        public AuthService(IAuthRepositories authRepositories, IConfiguration configuration)
        {
            _authRepositories = authRepositories;
            _configuration = configuration;
        }

        public async Task<(string? accessToken, string? refreshToken)> LoginAsync(string email, string password, string ipAddress)
        {
            var user = await _authRepositories.GetUserByEmailAsync(email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return (null, null);
            }

            var accessToken = GenerateAccessToken(user);
            var refreshToken = CreateRefreshToken(user, ipAddress);

            await _authRepositories.AddRefreshTokenAsync(refreshToken);

            return (accessToken, refreshToken.Token);
        }

        public async Task<(string? accessToken, string? refreshToken)> LoginAsync(string email, string ipAddress)
        {
            var user = await _authRepositories.GetUserByEmailAsync(email);
            if (user == null)
            {
                return (null, null);
            }

            var accessToken = GenerateAccessToken(user);
            var refreshToken = CreateRefreshToken(user, ipAddress);

            await _authRepositories.AddRefreshTokenAsync(refreshToken);

            return (accessToken, refreshToken.Token);
        }

        public async Task<(string? accessToken, string? newRefreshToken)> RefreshTokenAsync(string token, string ipAddress)
        {
            var refreshToken = await _authRepositories.GetRefreshTokenByTokenAsync(token);

            if (refreshToken == null || !refreshToken.IsActive)
            {
                return (null, null);
            }

            // Token rotasyonu
            var newRefreshToken = CreateRefreshToken(refreshToken.User, ipAddress);

            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            refreshToken.ReplacedByToken = newRefreshToken.Token;

            // İki işlemi tek transaction'da yapmak daha güvenli olabilir ama bu da iş görür.
            await _authRepositories.UpdateRefreshTokenAsync(refreshToken);
            await _authRepositories.AddRefreshTokenAsync(newRefreshToken);

            var newAccessToken = GenerateAccessToken(refreshToken.User);
            return (newAccessToken, newRefreshToken.Token);
        }

        public async Task<bool> RevokeTokenAsync(string token, string ipAddress)
        {
            var refreshToken = await _authRepositories.GetRefreshTokenByTokenAsync(token);
            if (refreshToken == null || !refreshToken.IsActive) return false;

            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;

            await _authRepositories.UpdateRefreshTokenAsync(refreshToken);
            return true;
        }
        public async Task<(bool Success, IEnumerable<string> Errors)> RegisterAsync(string firstName, string lastName, string email, string password)
        {
            Random Random = new Random();
            // 1. Kullanıcının zaten var olup olmadığını kontrol et.
            var userExists = await _authRepositories.UserExistsAsync(email);
            if (userExists)
            {
                return (false, new[] { "Bu e-posta adresi zaten kullanılıyor." });
            }
            // 2. Yeni bir User nesnesi oluştur.
            var user = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Roles = "Student" 
            };

            var student = new Student
            {
                SchoolNumber = Random.Next(1,1000), 
            };

            // 4. Kullanıcıyı veritabanına ekle.
            await _authRepositories.AddUserAsync(user, student);
            return (true, Enumerable.Empty<string>());
        }

        // --- Yardımcı Metotlar (GenerateAccessToken ve CreateRefreshToken) ---
        // Bu metotlar önceki cevapla aynıdır, sadece Student -> User değişikliği yapılmıştır.
        private string GenerateAccessToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Roles),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(_configuration["Jwt:AccessTokenExpirationMinutes"])),
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private RefreshToken CreateRefreshToken(User user, string ipAddress)
        {
            using var rng = RandomNumberGenerator.Create();
            var randomBytes = new byte[64];
            rng.GetBytes(randomBytes);
            return new RefreshToken
            {
                UserId = user.UserId,
                Token = Convert.ToBase64String(randomBytes),
                Expires = DateTime.UtcNow.AddDays(Convert.ToInt32(_configuration["Jwt:RefreshTokenExpirationDays"])),
                Created = DateTime.UtcNow,
                CreatedByIp = ipAddress,
            };
        }
    }
}
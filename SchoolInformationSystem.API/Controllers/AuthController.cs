using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SchoolInformationSystem.Application.DTOs;
using SchoolInformationSystem.Application.Interfaces;
using SchoolInformationSystem.Application.Services;
using SchoolInformationSystem.Infrastructure.Repositories;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SchoolInformationSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;
        public AuthController(IAuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            var ipAddress = GetIpAddress();
            var (accessToken, refreshToken) = await _authService.LoginAsync(model.Email, model.Password, ipAddress);

            if (string.IsNullOrEmpty(accessToken)) return Unauthorized("Invalid credentials.");

            SetRefreshTokenInCookie(refreshToken!);
            return Ok(new { AccessToken = accessToken });
        }

        [Authorize]
        [HttpGet("exa")]
        public async Task<IActionResult> Example()
        {
            

            return Ok(new { AccessToken = "adasdas" });
        }


        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken)) return Unauthorized("Refresh token not found.");

            var ipAddress = GetIpAddress();
            var (newAccessToken, newRefreshToken) = await _authService.RefreshTokenAsync(refreshToken, ipAddress);

            if (string.IsNullOrEmpty(newAccessToken)) return Unauthorized("Invalid refresh token.");

            SetRefreshTokenInCookie(newRefreshToken!);
            return Ok(new { AccessToken = newAccessToken });
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (!string.IsNullOrEmpty(refreshToken))
            {
                var ipAddress = GetIpAddress();
                await _authService.RevokeTokenAsync(refreshToken, ipAddress);
            }
            Response.Cookies.Delete("refreshToken");
            return Ok(new { message = "Logged out" });
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (success, errors) = await _authService.RegisterAsync(model.FirstName,model.LastName,model.Email,model.Password);

            if (!success)
            {
                // Servisten gelen hataları BadRequest olarak döndür.
                return BadRequest(new { Errors = errors });
            }

            return Ok(new { Message = "Kayıt başarılı. Lütfen giriş yapın." });
        }
        private void SetRefreshTokenInCookie(string refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(_configuration.GetValue<int>("Jwt:RefreshTokenExpirationDays")),
                SameSite = SameSiteMode.Strict,
                Secure = true,
                Path = "/api/auth" // Güvenlik için cookie'yi sadece bu path'e gönder.
            };
            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }

        private string GetIpAddress() => HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }
}


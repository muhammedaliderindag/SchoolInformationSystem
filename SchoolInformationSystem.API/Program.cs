using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SchoolInformationSystem.Application.Interfaces;
using SchoolInformationSystem.Application.Services;
using SchoolInformationSystem.Infrastructure.Data;
using SchoolInformationSystem.Infrastructure.Repositories;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
// Add services to the container.

// 1. Connection string'i appsettings.json dosyas�ndan oku.
//    "DefaultConnection" ismi, appsettings.json'daki isimle ayn� olmal�.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var configuration = builder.Configuration;
// 2. Servisleri konteynere ekle.


// 3. DbContext'i servis olarak ekle ve ona connection string'i ver.
//    Bu sat�r, EF Core'a hangi veritaban�n� (SQL Server) ve hangi ba�lant� dizesini 
//    kullanaca��n� s�yler.
builder.Services.AddDbContext<SchoolInformationSystemDbContext>(options =>
    options.UseSqlServer(connectionString));





// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddScoped<IAuthRepositories, AuthRepositories>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("https://localhost:7214", // <<-- Client'�n�z�n HTTPS adresi
                                             "http://localhost:5122")  // <<-- Client'�n�z�n HTTP adresi
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});

// 3. Authentication Yap�land�rmas�
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = configuration["Jwt:Issuer"],
        ValidAudience = configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
    };
});


builder.Services.AddEndpointsApiExplorer();


var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors(MyAllowSpecificOrigins);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

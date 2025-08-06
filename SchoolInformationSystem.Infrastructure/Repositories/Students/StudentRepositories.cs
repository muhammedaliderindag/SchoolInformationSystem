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

        public async Task<List<LessonList>> GetLessonsAsyncRepo(int userId)
        {
            // Tüm dersleri tek bir sorgu ile çekiyoruz.
            // 'Select' ifadesi içinde, öğrencinin o dersi seçip seçmediğini kontrol ediyoruz.
            var lessons = await _context.Lessons
                .Include(lesson => lesson.Teacher)
                    .ThenInclude(teacher => teacher.User)
                // StudentSelectedLessons tablosunu da sorguya dahil ediyoruz ki
                // öğrencinin dersi seçip seçmediğini kontrol edebilelim.
                .Include(lesson => lesson.StudentSelectedLessons)
                .AsNoTracking()
                .Select(lesson => new LessonList
                {
                    LessonId = lesson.LessonId,
                    LessonName = lesson.LessonName,

                    // Burası en önemli kısım:
                    // O anki dersin 'StudentSelectedLessons' listesinde, verilen 'userId' ile eşleşen
                    // bir kayıt var mı diye kontrol ediyoruz.
                    // Varsa 'Added' = 1, yoksa 'Added' = 0 olarak ayarlanıyor.
                    Added = lesson.StudentSelectedLessons.Any(ssl => ssl.StudentId == userId) ? 1 : 0,

                    TeacherName = lesson.Teacher.User.FirstName + " " + lesson.Teacher.User.LastName,
                    Credit = lesson.Credit ?? 0,
                    Akts = lesson.Akts ?? 0,
                    TeacherId = lesson.Teacher.TeacherId,
                    ClassId = lesson.Derslik ?? 0
                })
                .ToListAsync();

            return lessons;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
        }

        // Parametreyi List<Lesson> olarak düzeltiyoruz. Bu daha mantıklı.
        public async Task<ServiceResponse<string>> SaveSelectedLessonsRepo(List<LessonList> lessons, int UserId)
        {
            // 1. Gelen listenin boş olup olmadığını kontrol et.
            if (lessons == null || !lessons.Any())
            {
                // Başarılı ama yapılacak bir şey yok veya hata olarak dönebilirsiniz.
                return ServiceResponse<string>.Success("Kaydedilecek ders bulunamadı.");
            }

            // 2. Veritabanına eklenecek nesnelerin bir listesini oluştur.
            var lessonsToSave = new List<StudentSelectedLesson>();

            foreach (var item in lessons)
            {
                var queryMapping = new StudentSelectedLesson
                {
                    StudentId = UserId,
                    LessonId = item.LessonId,
                    TeacherId = item.TeacherId 
                };
                lessonsToSave.Add(queryMapping);
            }

            try
            {
                // 3. Tüm listeyi tek seferde context'e ekle. Bu, Add'den daha performanslıdır.
                await _context.StudentSelectedLessons.AddRangeAsync(lessonsToSave);

                // 4. Tüm değişiklikleri tek bir veritabanı işleminde (transaction) kaydet.
                await _context.SaveChangesAsync();

                return ServiceResponse<string>.Success("Veri başarıyla database kaydedildi.");
            }
            catch (Exception ex)
            {
                // Hata durumunda loglama yapabilir ve istemciye anlamlı bir hata dönebilirsiniz.
                // Örneğin: _logger.LogError(ex, "Ders kaydı sırasında hata oluştu.");
                return ServiceResponse<string>.Fail($"Kayıt sırasında bir hata oluştu: {ex.Message}");
            }
        }
    }
}

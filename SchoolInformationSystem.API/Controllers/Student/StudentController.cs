using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolInformationSystem.Application.DTOs;
using SchoolInformationSystem.Application.Interfaces.IStudent;

namespace SchoolInformationSystem.API.Controllers.Student
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _Student;
        public StudentController(IStudentService Student)
        {
            _Student = Student;
        }

        [HttpGet("getLessons")]
        public async Task<IActionResult> GetLessons([FromQuery] int UserId)
        {
            var response = await _Student.GetLessonsAsync(UserId);
            if (response == null)
            {
                return NotFound("No lessons found.");
            }
            return Ok(response);
        }
        [HttpPost("saveSelectedLessons")]
        public async Task<IActionResult> SaveSelectedLessonsController([FromBody] List<LessonList> list, [FromQuery] int UserId)
        {
            var response = await _Student.SaveSelectedLessons(list, UserId);
            if (!response.IsSuccess)
            {
                return  BadRequest(response);
            }
            return Ok(response);
        }
    }
}

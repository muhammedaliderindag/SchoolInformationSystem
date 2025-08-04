using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> GetLessons()
        {
            var response = await _Student.GetLessonsAsync();
            if (response == null)
            {
                return NotFound("No lessons found.");
            }
            return Ok(response);
        }
    }
}

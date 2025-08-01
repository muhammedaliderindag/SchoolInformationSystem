using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolInformationSystem.Application.DTOs;
using SchoolInformationSystem.Application.Interfaces;
using SchoolInformationSystem.Domain.Entities;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SchoolInformationSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class StudentsOverviewController : ControllerBase
    {

        private readonly IStudentsOverviewService _StudentsOverview;
        public StudentsOverviewController(IStudentsOverviewService StudentsOverview)
        {
            _StudentsOverview = StudentsOverview;
        }
        [HttpGet("students")]
        public async Task<List<StudentsDto>> Get()
        {
            var response = await _StudentsOverview.GetStudentsAsync();
            return (List<StudentsDto>)response;
        }
        [HttpGet("{UserId}")]
        public async Task<StudentsDto> GetStudentsFromId(int UserId)
        {
            var students = await _StudentsOverview.GetStudentFromIdAsync(UserId);
            if (students == null)
            {
                return null;
            }
            return (StudentsDto)students;
        }

        [HttpPost("Update")]
        public async Task<IActionResult> StudentEditPageUpdate([FromBody] UpdateStudentDto model)
        {
            var response = await _StudentsOverview.StudentEditPageUpdateService(model.UserId, model.NewSchoolNum, model.Email);
            if (!response.IsSuccess)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.Data);
        }
    }
}

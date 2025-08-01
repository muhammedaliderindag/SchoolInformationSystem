using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolInformationSystem.Application.DTOs
{
    public class UpdateStudentDto
    {
        public int UserId { get; set; }
        public int NewSchoolNum { get; set; }
        public string Email { get; set; } = string.Empty;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolInformationSystem.Application.DTOs
{
    public class StudentsDto
    {
        public int? UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int? SchoolNumber { get; set; }
        public string? Email { get; set; }
        public string? Roles { get; set; }
    }
}

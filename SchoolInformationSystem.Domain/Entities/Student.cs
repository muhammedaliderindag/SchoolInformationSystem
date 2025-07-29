using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SchoolInformationSystem.Domain.Entities;

public partial class Student
{
    [Key]
    public int StudentId { get; set; }

    public int SchoolNumber { get; set; }
}

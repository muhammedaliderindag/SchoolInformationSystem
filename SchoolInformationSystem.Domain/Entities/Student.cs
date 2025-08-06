using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SchoolInformationSystem.Domain.Entities;

[Index("UserId", Name = "IX_Students", IsUnique = true)]
public partial class Student
{
    [Key]
    public int StudentId { get; set; }

    public int SchoolNumber { get; set; }

    public int UserId { get; set; }

    [InverseProperty("Student")]
    public virtual ICollection<StudentSelectedLesson> StudentSelectedLessons { get; set; } = new List<StudentSelectedLesson>();

    [ForeignKey("UserId")]
    [InverseProperty("Student")]
    public virtual User User { get; set; } = null!;
}

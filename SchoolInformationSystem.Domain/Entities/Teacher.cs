using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SchoolInformationSystem.Domain.Entities;

[Index("UserId", Name = "IX_Teachers", IsUnique = true)]
[Index("LessonId", Name = "IX_Teachers_1", IsUnique = true)]
public partial class Teacher
{
    [Key]
    public int TeacherId { get; set; }

    public int? LessonId { get; set; }

    public int UserId { get; set; }

    [ForeignKey("LessonId")]
    [InverseProperty("Teacher")]
    public virtual Lesson? Lesson { get; set; }

    [InverseProperty("Teacher")]
    public virtual ICollection<StudentSelectedLesson> StudentSelectedLessons { get; set; } = new List<StudentSelectedLesson>();

    [ForeignKey("UserId")]
    [InverseProperty("Teacher")]
    public virtual User User { get; set; } = null!;
}

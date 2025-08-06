using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SchoolInformationSystem.Domain.Entities;

[Table("StudentSelectedLesson")]
public partial class StudentSelectedLesson
{
    [Key]
    public int Id { get; set; }

    public int StudentId { get; set; }

    public int LessonId { get; set; }

    public int TeacherId { get; set; }

    [ForeignKey("LessonId")]
    [InverseProperty("StudentSelectedLessons")]
    public virtual Lesson Lesson { get; set; } = null!;

    [ForeignKey("StudentId")]
    [InverseProperty("StudentSelectedLessons")]
    public virtual Student Student { get; set; } = null!;

    [ForeignKey("TeacherId")]
    [InverseProperty("StudentSelectedLessons")]
    public virtual Teacher Teacher { get; set; } = null!;
}

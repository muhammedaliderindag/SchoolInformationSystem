using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SchoolInformationSystem.Domain.Entities;

[Keyless]
[Table("StudentSelectedLesson")]
public partial class StudentSelectedLesson
{
    public int StudentId { get; set; }

    public int LessonId { get; set; }

    public int TeacherId { get; set; }

    [ForeignKey("LessonId")]
    public virtual Lesson Lesson { get; set; } = null!;
}

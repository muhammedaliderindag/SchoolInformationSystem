using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SchoolInformationSystem.Domain.Entities;

public partial class Lesson
{
    [Key]
    public int LessonId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string LessonName { get; set; } = null!;

    public int? Credit { get; set; }

    [Column("AKTS")]
    public int? Akts { get; set; }

    public int? Derslik { get; set; }

    [InverseProperty("Lesson")]
    public virtual ICollection<StudentSelectedLesson> StudentSelectedLessons { get; set; } = new List<StudentSelectedLesson>();

    [InverseProperty("Lesson")]
    public virtual Teacher? Teacher { get; set; }
}

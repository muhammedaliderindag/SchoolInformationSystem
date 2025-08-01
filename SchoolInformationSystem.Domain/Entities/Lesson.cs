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
}

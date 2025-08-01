using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SchoolInformationSystem.Domain.Entities;

[Index("UserId", Name = "IX_Teachers", IsUnique = true)]
public partial class Teacher
{
    [Key]
    public int TeacherId { get; set; }

    public int? LessonId { get; set; }

    public int UserId { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Teacher")]
    public virtual User User { get; set; } = null!;
}

using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SchoolInformationSystem.Domain.Entities;

namespace SchoolInformationSystem.Infrastructure.Data;

public partial class SchoolInformationSystemDbContext : DbContext
{
    public SchoolInformationSystemDbContext(DbContextOptions<SchoolInformationSystemDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Lesson> Lessons { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<StudentSelectedLesson> StudentSelectedLessons { get; set; }

    public virtual DbSet<Teacher> Teachers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Lesson>(entity =>
        {
            entity.Property(e => e.LessonId).ValueGeneratedNever();
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RefreshTokens_Users");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasOne(d => d.User).WithOne(p => p.Student)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Students_Users");
        });

        modelBuilder.Entity<StudentSelectedLesson>(entity =>
        {
            entity.HasOne(d => d.Lesson).WithMany(p => p.StudentSelectedLessons)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StudentSelectedLesson_Lessons1");

            entity.HasOne(d => d.Student).WithMany(p => p.StudentSelectedLessons)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StudentSelectedLesson_Students");

            entity.HasOne(d => d.Teacher).WithMany(p => p.StudentSelectedLessons)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StudentSelectedLesson_Teachers");
        });

        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.HasOne(d => d.Lesson).WithOne(p => p.Teacher).HasConstraintName("FK_Teachers_Lessons");

            entity.HasOne(d => d.User).WithOne(p => p.Teacher)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Teachers_Users");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.UserId).ValueGeneratedNever();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

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

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        OnModelCreatingPartial(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.UserId);
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.Email).IsRequired();
            entity.Property(u => u.PasswordHash).IsRequired();
        });

        // RefreshToken Tablosu Yapılandırması
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(rt => rt.Id);
            entity.Property(rt => rt.Token).IsRequired();

            // İlişki Tanımı (User modeline göre güncellendi)
            entity.HasOne(rt => rt.User)
                  .WithMany(u => u.RefreshTokens)
                  .HasForeignKey(rt => rt.UserId);
        });
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

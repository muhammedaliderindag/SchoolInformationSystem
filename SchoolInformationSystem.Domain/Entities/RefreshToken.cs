using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SchoolInformationSystem.Domain.Entities;

[Index("Token", Name = "IX_RefreshTokens_Token")]
[Index("UserId", Name = "IX_RefreshTokens_UserId")]
public partial class RefreshToken
{
    [Key]
    public int Id { get; set; }

    [StringLength(512)]
    public string Token { get; set; } = null!;

    public DateTime Expires { get; set; }

    public bool IsActive => DateTime.UtcNow <= Expires && Revoked == null;

    public DateTime Created { get; set; }

    [StringLength(45)]
    public string? CreatedByIp { get; set; }

    public DateTime? Revoked { get; set; }

    [StringLength(45)]
    public string? RevokedByIp { get; set; }

    [StringLength(512)]
    public string? ReplacedByToken { get; set; }

    public int UserId { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("RefreshTokens")]
    public virtual User User { get; set; } = null!;
}

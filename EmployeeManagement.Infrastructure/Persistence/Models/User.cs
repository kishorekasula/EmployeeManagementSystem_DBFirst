using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Infrastructure.Persistence.Models;

[Table("Users", Schema = "Security")]
public partial class User
{
    [Key]
    public int UserId { get; set; }

    [StringLength(100)]
    public string FirstName { get; set; } = null!;

    [StringLength(100)]
    public string LastName { get; set; } = null!;

    [StringLength(255)]
    public string Email { get; set; } = null!;

    [StringLength(500)]
    public string PasswordHash { get; set; } = null!;

    public bool IsEmailVerified { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? LastLoginAt { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<EmailOtp> EmailOtps { get; set; } = new List<EmailOtp>();

    [InverseProperty("User")]
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    [InverseProperty("User")]
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}

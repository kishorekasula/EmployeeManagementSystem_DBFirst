using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Infrastructure.Persistence.Models;

[Table("EmailOtps", Schema = "Security")]
[Index("UserId", Name = "IX_EmailOtps_UserId")]
public partial class EmailOtp
{
    [Key]
    public long EmailOtpId { get; set; }

    public int UserId { get; set; }

    [StringLength(255)]
    public string OtpHash { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    public bool IsUsed { get; set; }

    public int AttemptCount { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UsedAt { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("EmailOtps")]
    public virtual User User { get; set; } = null!;
}

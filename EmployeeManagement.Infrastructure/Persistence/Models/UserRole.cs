using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Infrastructure.Persistence.Models;

[Table("UserRoles", Schema = "Security")]
[Index("UserId", "RoleId", Name = "UQ_UserRoles_UserId_RoleId", IsUnique = true)]
public partial class UserRole
{
    [Key]
    public int UserRoleId { get; set; }

    public int UserId { get; set; }

    public int RoleId { get; set; }

    public DateTime AssignedAt { get; set; }

    [ForeignKey("RoleId")]
    [InverseProperty("UserRoles")]
    public virtual Role Role { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("UserRoles")]
    public virtual User User { get; set; } = null!;
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Infrastructure.Persistence.Models;

[Table("Roles", Schema = "Security")]
[Index("RoleCode", Name = "UQ_Roles_RoleCode", IsUnique = true)]
[Index("RoleName", Name = "UQ_Roles_RoleName", IsUnique = true)]
public partial class Role
{
    [Key]
    public int RoleId { get; set; }

    [StringLength(50)]
    public string RoleCode { get; set; } = null!;

    [StringLength(100)]
    public string RoleName { get; set; } = null!;

    [StringLength(500)]
    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [InverseProperty("Role")]
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}

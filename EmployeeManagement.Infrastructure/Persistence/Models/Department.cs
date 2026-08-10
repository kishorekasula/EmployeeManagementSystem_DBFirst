using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Infrastructure.Persistence.Models;

[Table("Departments", Schema = "Security")]
[Index("DepartmentCode", Name = "UQ_Departments_DepartmentCode", IsUnique = true)]
[Index("DepartmentName", Name = "UQ_Departments_DepartmentName", IsUnique = true)]
public partial class Department
{
    [Key]
    [Column("department_id")]
    public int DepartmentId { get; set; }

    [Column("department_code")]
    [StringLength(50)]
    public string DepartmentCode { get; set; } = null!;

    [Column("department_name")]
    [StringLength(100)]
    public string DepartmentName { get; set; } = null!;

    [Column("department_description")]
    [StringLength(500)]
    public string? DepartmentDescription { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Application.DTOs.Departments
{
    public class UpdateDepartmentRequestDto
    {
        public string department_code { get; set; } = string.Empty;
        public string department_name { get; set; } = string.Empty;
        public string? department_description { get; set; }
        public bool is_active { get; set; }
    }
}

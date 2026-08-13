using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Application.DTOs.Roles
{
    public class RoleResponseDto
    {
        public int role_id { get; set; }
        public string role_code { get; set; } = string.Empty;
        public string role_name { get; set; } = string.Empty;
        public string? description { get; set; }
        public bool is_active { get; set; }
        public DateTime created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }
}

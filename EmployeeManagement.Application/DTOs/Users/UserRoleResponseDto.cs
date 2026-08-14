using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Application.DTOs.Users
{
    public class UserRoleResponseDto
    {
        public int user_role_id { get; set; }
        public int user_id { get; set; }
        public int role_id { get; set; }
        public string role_code { get; set; } = string.Empty;
        public string role_name { get; set; } = string.Empty;
        public DateTime assigned_at { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Application.DTOs.Users
{
    public class ChangePasswordRequestDto
    {
        public string current_password { get; set; } = string.Empty;
        public string new_password { get; set; } = string.Empty;
        public string confirm_password { get; set; } = string.Empty;
    }
}

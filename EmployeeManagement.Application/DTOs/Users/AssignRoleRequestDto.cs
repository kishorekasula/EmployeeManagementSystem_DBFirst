using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Application.DTOs.Users
{
    public class AssignRoleRequestDto
    {
        public int user_id { get; set; }
        public int role_id { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Application.DTOs.Auth
{
    public class LoginResponseDto
    {
        public int user_id { get; set; }
        public string first_name { get; set; } = string.Empty;
        public string last_name { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public List<string> role { get; set; } = [];
        public string token { get; set; } = string.Empty;
        //public DateTime AccessTokenExpiry { get; set; }
        //public string RefreshToken { get; set; } = string.Empty;
        //public DateTime RefreshTokenExpiry { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Application.DTOs.Auth
{
    public class VerifyResetOtpRequestDto
    {
        public string email { get; set; } = string.Empty;
        public string otp { get; set; } = string.Empty;
    }
}

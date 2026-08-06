using EmployeeManagement.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Application.Interfaces.Services
{
    public interface IJwtService
    {
        string GetAccessToken(JwtUserDto user, List<string> roles);

        string GetRefreshToken();

        DateTime GetAccessTokenExpiry();

        DateTime GetRefreshTokenExpiry();
    }
}

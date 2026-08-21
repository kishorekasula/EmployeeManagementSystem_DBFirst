using EmployeeManagement.Application.DTOs.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Application.Interfaces.Services
{
    public interface IUserRoleService
    {
        Task<List<UserRoleResponseDto>> GetUserRolesAsync(int userId);
        Task<UserRoleResponseDto> AssignRoleAsync(int userId, AssignRoleRequestDto request);

        Task<bool> RemoveRoleAsync(int userId, int roleId);
    }
}

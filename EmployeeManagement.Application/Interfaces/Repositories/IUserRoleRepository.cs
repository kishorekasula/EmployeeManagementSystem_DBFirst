using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeManagement.Application.DTOs.Users;

namespace EmployeeManagement.Application.Interfaces.Repositories
{
    public interface IUserRoleRepository
    {
        Task<UserRoleResponseDto?> AssignRoleAsync(int userId, AssignRoleRequestDto request);
        Task<List<UserRoleResponseDto>> GetUserRolesAsync(int userId);
        Task<bool> RemoveRoleAsync(int userId, int roleId);
        Task<bool> RoleAlreadyAssignedAsync(int userId, int roleId);
        Task<bool> HasRoleAsync(int userId, int roleId);
        Task<bool> UserExistsAsync(int userId);
        Task<bool> RoleExistsAsync(int roleId);
    }
}

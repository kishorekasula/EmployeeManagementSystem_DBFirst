using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeManagement.Application.DTOs.Roles;

namespace EmployeeManagement.Application.Interfaces.Repositories
{
    public interface IRoleRepository
    {
        Task<List<RoleResponseDto>> GetAllRolesAsync();
        Task<RoleResponseDto?> GetRoleByIdAsync(int id);
        Task<bool> ExistsByCodeAsync(string roleCode);
        Task<bool> ExistsByNameAsync(string roleName);
        Task<RoleResponseDto> CreateRoleAsync(CreateRoleRequestDto request);
        Task<RoleResponseDto> UpdateRoleAsync(int roleId, UpdateRoleRequestDto request);
        Task<bool> DeleteRoleAsync(int roleId);
    }
}

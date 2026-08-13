using EmployeeManagement.Application.DTOs.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Application.Interfaces.Services;

public interface IRoleService
{
    Task<List<RoleResponseDto>> GetAllRolesAsync();
    Task<RoleResponseDto?> GetRoleByIdAsync(int id);
    Task<RoleResponseDto> CreateRoleAsync(CreateRoleRequestDto request);
    Task<RoleResponseDto> UpdateRoleAsync(int roleId, UpdateRoleRequestDto request);
    Task DeleteRoleAsync(int roleId);
}

using EmployeeManagement.Application.DTOs.Roles;

namespace EmployeeManagement.Application.Interfaces.Services;

public interface IRoleService
{
    Task<List<RoleResponseDto>> GetAllRolesAsync();
    Task<RoleResponseDto?> GetRoleByIdAsync(int id);
    Task<RoleResponseDto> CreateRoleAsync(CreateRoleRequestDto request);
    Task<RoleResponseDto> UpdateRoleAsync(int roleId, UpdateRoleRequestDto request);
    Task DeleteRoleAsync(int roleId);
}

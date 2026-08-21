using EmployeeManagement.Application.DTOs.Users;
using EmployeeManagement.Application.Interfaces.Repositories;
using EmployeeManagement.Application.Interfaces.Services;

namespace EmployeeManagement.Application.Services;

public class UserRoleService : IUserRoleService
{
    private readonly IUserRoleRepository _userRoleRepository;

    public UserRoleService(IUserRoleRepository userRoleRepository)
    {
        _userRoleRepository = userRoleRepository;
    }

    public async Task<List<UserRoleResponseDto>> GetUserRolesAsync(int userId)
    {
        var userExists = await _userRoleRepository.UserExistsAsync(userId);

        if (!userExists)
        {
            throw new KeyNotFoundException(
                $"User with ID {userId} not found.");
        }

        return await _userRoleRepository.GetUserRolesAsync(userId);
    }

    public async Task<UserRoleResponseDto> AssignRoleAsync(int userId, AssignRoleRequestDto request)
    {
        var userExists = await _userRoleRepository.UserExistsAsync(userId);

        if (!userExists)
        {
            throw new KeyNotFoundException( $"User with ID {userId} not found.");
        }

        var roleExists = await _userRoleRepository.RoleExistsAsync(request.role_id);

        if (!roleExists)
        {
            throw new KeyNotFoundException($"Role with ID {request.role_id} not found.");
        }

        var alreadyAssigned = await _userRoleRepository.HasRoleAsync(userId, request.role_id);

        if (alreadyAssigned)
        {
            throw new InvalidOperationException("This role is already assigned to the user.");
        }
        await _userRoleRepository.AssignRoleAsync(userId, request);

        var roles = await _userRoleRepository.GetUserRolesAsync(userId);

        var assignedRole = roles.FirstOrDefault(x => x.role_id == request.role_id);

        if (assignedRole == null)
        {
            throw new InvalidOperationException("Role assignment failed.");
        }

        return assignedRole;
    }

    public async Task<bool> RemoveRoleAsync(int userId, int roleId)
    {
        var userExists = await _userRoleRepository.UserExistsAsync(userId);

        if (!userExists)
        {
            throw new KeyNotFoundException($"User with ID {userId} not found.");
        }

        var roleExists = await _userRoleRepository.RoleExistsAsync(roleId);

        if (!roleExists)
        {
            throw new KeyNotFoundException($"Role with ID {roleId} not found.");
        }

        var assigned = await _userRoleRepository.HasRoleAsync(userId, roleId);

        if (!assigned)
        {
            throw new InvalidOperationException("This role is not assigned to the user.");
        }

        return await _userRoleRepository.RemoveRoleAsync(userId, roleId);
    }
}
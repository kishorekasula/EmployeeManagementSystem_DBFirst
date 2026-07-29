using EmployeeManagement.Application.DTOs.Users;
using EmployeeManagement.Application.Interfaces.Repositories;
using EmployeeManagement.Application.Interfaces.Services;

namespace EmployeeManagement.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<List<UserResponseDto>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();

        return users.Select(MapToResponse).ToList();
    }

    public async Task<UserResponseDto?> GetByIdAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
            return null;

        return MapToResponse(user);
    }

    private static UserResponseDto MapToResponse(UserDataDto user)
    {
        return new UserResponseDto
        {
            UserId = user.UserId,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            IsEmailVerified = user.IsEmailVerified,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            LastLoginAt = user.LastLoginAt,
            Roles = user.Roles
        };
    }
}
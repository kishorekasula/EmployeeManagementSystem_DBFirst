using EmployeeManagement.Application.DTOs.Users;

namespace EmployeeManagement.Application.Interfaces.Services;

public interface IUserService
{
    Task<List<UserResponseDto>> GetAllAsync();
    Task<UserResponseDto?> GetByIdAsync(int userId);
    Task<UserResponseDto> CreateAsync(CreateUserRequestDto request);
    Task<UserResponseDto?> UpdateAsync(int userId, UpdateUserRequestDto request);
    Task<bool> DeleteAsync(int userId);
}
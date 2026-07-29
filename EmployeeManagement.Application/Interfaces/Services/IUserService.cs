using EmployeeManagement.Application.DTOs.Users;

namespace EmployeeManagement.Application.Interfaces.Services;

public interface IUserService
{
    Task<List<UserResponseDto>> GetAllAsync();

    Task<UserResponseDto?> GetByIdAsync(int userId);
}
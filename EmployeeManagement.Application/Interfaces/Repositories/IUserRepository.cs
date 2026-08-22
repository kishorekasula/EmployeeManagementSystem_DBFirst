using EmployeeManagement.Application.DTOs.Auth;
using EmployeeManagement.Application.DTOs.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<List<UserDataDto>> GetAllAsync();
        Task<UserDataDto?> GetByIdAsync(int userId);
        Task<UserDataDto?> GetByEmailAsync(string email);
        Task<bool> EmailExistsAsync(string email);
        Task<int> CreateAsync(CreateUserDataDto user, string roleCode);
        Task<UserDataDto> CreateUserAsync(CreateUserRequestDto request, string passwordHash);
        Task<UserDataDto?> UpdateAsync(int userId, UpdateUserRequestDto request);
        Task<bool> DeleteAsync(int userId);
        Task MarkEmailAsVerifiedAsync(int userId);
        Task<LoginUserDto?> GetLoginUserAsync(string email);
        Task UpdateLastLoginAsync(int userId);
        Task<string?> GetPasswordHashAsync(int userId);
        Task UpdatePasswordAsync(int userId, string passwordHash);
    }
}

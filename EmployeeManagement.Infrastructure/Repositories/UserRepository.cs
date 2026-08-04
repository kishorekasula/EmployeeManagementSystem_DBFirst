using EmployeeManagement.Application.DTOs.Users;
using EmployeeManagement.Application.Interfaces.Repositories;
using EmployeeManagement.Infrastructure.Data;
using EmployeeManagement.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly EmployeeManagementDbContext _dbContext;

    public UserRepository(EmployeeManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<UserDataDto>> GetAllAsync()
    {
        return await _dbContext.Users
            .AsNoTracking()
            .Select(user => new UserDataDto
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

                Roles = user.UserRoles
                    .Select(ur => ur.Role.RoleName)
                    .ToList()
            })
            .OrderBy(user => user.FirstName)
            .ThenBy(user => user.LastName)
            .ToListAsync();
    }

    public async Task<UserDataDto?> GetByIdAsync(int userId)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .Where(user => user.UserId == userId)
            .Select(user => new UserDataDto
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

                Roles = user.UserRoles
                    .Select(ur => ur.Role.RoleName)
                    .ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<UserDataDto?> GetByEmailAsync(string email)
    {
        email = email.Trim();

        return await _dbContext.Users.AsNoTracking().Where(user => user.Email == email).Select(user => new UserDataDto
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

                Roles = user.UserRoles.Select(ur => ur.Role.RoleName).ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        email = email.Trim();

        return await _dbContext.Users
            .AsNoTracking()
            .AnyAsync(user => user.Email == email);
    }

    public async Task<int> CreateAsync(CreateUserDataDto userDto, string roleCode)
    {
        var role = await _dbContext.Roles
            .FirstOrDefaultAsync(x =>
                x.RoleCode == roleCode &&
                x.IsActive);

        if (role == null)
        {
            throw new InvalidOperationException(
                $"Active role '{roleCode}' was not found.");
        }

        var user = new User
        {
            FirstName = userDto.FirstName,
            LastName = userDto.LastName,
            Email = userDto.Email,
            PasswordHash = userDto.PasswordHash,
            IsEmailVerified = false,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        user.UserRoles.Add(new UserRole
        {
            RoleId = role.RoleId,
            AssignedAt = DateTime.UtcNow
        });

        _dbContext.Users.Add(user);

        await _dbContext.SaveChangesAsync();

        return user.UserId;
    }

    public async Task MarkEmailAsVerifiedAsync(int userId)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserId == userId);

        if (user == null)
            return;

        user.IsEmailVerified = true;
        user.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
    }
}
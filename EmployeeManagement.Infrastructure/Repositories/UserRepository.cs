using EmployeeManagement.Application.DTOs.Auth;
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
        return await _dbContext.Users.AsNoTracking()
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

    public async Task<LoginUserDto?> GetLoginUserAsync(string email)
    {
        email = email.Trim();

        return await _dbContext.Users.AsNoTracking().Where(x => x.Email == email).Select(user => new LoginUserDto
            {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PasswordHash = user.PasswordHash,
                IsEmailVerified = user.IsEmailVerified,
                IsActive = user.IsActive,

                Roles = user.UserRoles.Select(r => r.Role.RoleName).ToList()
            }).FirstOrDefaultAsync();
    }

    public async Task UpdateLastLoginAsync(int userId)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserId == userId);

        if (user == null)
            return;

        user.LastLoginAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

    }

    public async Task<UserDataDto> CreateUserAsync(CreateUserRequestDto request, string passwordHash)
    {
        var role = await _dbContext.Roles.FirstOrDefaultAsync(r => r.RoleCode == request.role_code && r.IsActive);

        if (role == null)
        {
            throw new InvalidOperationException($"Active role '{request.role_code}' was not found.");
        }

        var user = new User
        {
            FirstName = request.first_name,
            LastName = request.last_name,
            Email = request.email.Trim(),
            PasswordHash = passwordHash,
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

        return await GetByIdAsync(user.UserId)?? throw new InvalidOperationException("Failed to retrieve the newly created user.");
    }

    public async Task<UserDataDto?> UpdateAsync(int userId, UpdateUserRequestDto request)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserId == userId);

        if (user == null)
        {
            return null;
        }

        user.FirstName = request.first_name.Trim();
        user.LastName = request.last_name.Trim();
        user.IsActive = request.is_active;
        user.Email = request.email.Trim();
        user.UpdatedAt = DateTime.Now;

        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(user.UserId);
    }

    public async Task<bool> DeleteAsync(int userId)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserId == userId);

        if (user == null)
        {
            return false;
        }

        user.IsActive = false;
        user.UpdatedAt = DateTime.Now;

        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<string?> GetPasswordHashAsync(int userId)
    {
        return await _dbContext.Users.AsNoTracking().Where(x => x.UserId == userId)
        .Select(x => x.PasswordHash)
        .FirstOrDefaultAsync();
    }

    public async Task UpdatePasswordAsync(int userId, string passwordHash)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserId == userId);

        if (user == null)
        {
            throw new InvalidOperationException($"User with ID '{userId}' was not found.");
        }

        user.PasswordHash = passwordHash;
        user.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
    }
}
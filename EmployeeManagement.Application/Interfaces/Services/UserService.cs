using EmployeeManagement.Application.DTOs.Users;
using EmployeeManagement.Application.Common.Exceptions;
using EmployeeManagement.Application.Interfaces.Repositories;
using EmployeeManagement.Application.Interfaces.Services;

namespace EmployeeManagement.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
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

    public static UserResponseDto MapToResponse(UserDataDto user)
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

    public async Task<UserResponseDto> CreateAsync(CreateUserRequestDto request)
    {
        var email = request.email.Trim().ToLowerInvariant();
        var firstName = request.first_name.Trim();
        var lastName = request.last_name.Trim();
        var roleCode = request.role_code.Trim().ToUpperInvariant();

        if (string.IsNullOrEmpty(firstName))
        {
            throw new ArgumentException("First name is required.", nameof(request.first_name));
        }

        if (string.IsNullOrEmpty(lastName))
        {
            throw new ArgumentException("Last name is required.", nameof(request.last_name));
        }

        if (string.IsNullOrEmpty(email))
        {
            throw new ArgumentException("Email is required.", nameof(request.email));
        }

        if (string.IsNullOrEmpty(request.password))
        {
            throw new ArgumentException("Password is required.", nameof(request.password));
        }

        if (string.IsNullOrEmpty(roleCode))
        {
            throw new ArgumentException("Role code is required.");
        }

        var emailExists = await _userRepository.EmailExistsAsync(email);

        if (emailExists)
        {
            throw new ConflictException($"Email '{email}' is already in use.");
        }

        var passwordHash = _passwordHasher.Hash(request.password);

        var userId = await _userRepository.CreateAsync(new CreateUserDataDto
        {
            FirstName = request.first_name,
            LastName = request.last_name,
            Email = email,
            PasswordHash = passwordHash
        }, request.role_code);

        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
        {
            throw new Exception("User could not be created.");
        }

        return MapToResponse(user);
    }
}
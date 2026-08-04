using EmployeeManagement.Application.DTOs.Auth;

namespace EmployeeManagement.Application.Interfaces.Services;

public interface IAuthService
{
    Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request);

    Task VerifyEmailAsync(VerifyEmailRequestDto request);

    Task ResendOtpAsync(ResendOtpRequestDto request);
}
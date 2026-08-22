using EmployeeManagement.Application.DTOs.Auth;

namespace EmployeeManagement.Application.Interfaces.Services;

public interface IAuthService
{
    Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request);
    Task VerifyEmailAsync(VerifyEmailRequestDto request);
    Task ResendOtpAsync(ResendOtpRequestDto request);
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
    Task ForgotPasswordAsync(string email);
    Task VerifyResetOtpAsync(string email, string otp);
    Task ResetPasswordAsync(string email, string otp, string new_password, string confirm_password);
}
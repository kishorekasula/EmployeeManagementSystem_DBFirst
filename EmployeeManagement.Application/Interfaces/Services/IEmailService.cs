namespace EmployeeManagement.Application.Interfaces.Services;

public interface IEmailService
{
    Task SendOtpAsync(string email, string firstName, string otp);
    Task SendOnboardingEmailAsync(string email, string firstName, string lastName, string? employeeCode = null);
}
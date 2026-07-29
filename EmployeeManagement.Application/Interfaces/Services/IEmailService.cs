namespace EmployeeManagement.Application.Interfaces.Services;

public interface IEmailService
{
    Task SendOtpAsync(string email, string firstName, string otp);
}
using EmployeeManagement.Application.DTOs.Auth;

namespace EmployeeManagement.Application.Interfaces.Repositories;

public interface IEmailOtpRepository
{
    Task CreateAsync(int userId,string otpHash,DateTime expiresAt);

    Task<EmailOtpDataDto?> GetLatestUnusedAsync(int userId);

    Task IncrementAttemptAsync( long emailOtpId);

    Task MarkAsUsedAsync( long emailOtpId);

    Task InvalidateUnusedOtpsAsync(int userId);
}
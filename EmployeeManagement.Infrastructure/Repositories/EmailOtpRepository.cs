using EmployeeManagement.Application.DTOs.Auth;
using EmployeeManagement.Application.Interfaces.Repositories;
using EmployeeManagement.Infrastructure.Data;
using EmployeeManagement.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Infrastructure.Repositories;

public class EmailOtpRepository : IEmailOtpRepository
{
    private readonly EmployeeManagementDbContext _dbContext;

    public EmailOtpRepository(EmployeeManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CreateAsync(int userId, string otpHash, DateTime expiresAt)
    {
        var entity = new EmailOtp
        {
            UserId = userId,
            OtpHash = otpHash,
            ExpiresAt = expiresAt,
            IsUsed = false,
            AttemptCount = 0,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.EmailOtps.Add(entity);

        await _dbContext.SaveChangesAsync();
    }

    public async Task<EmailOtpDataDto?> GetLatestUnusedAsync(int userId)
    {
        return await _dbContext.EmailOtps
            .AsNoTracking()
            .Where(x =>
                x.UserId == userId &&
                !x.IsUsed)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new EmailOtpDataDto
            {
                EmailOtpId = x.EmailOtpId,
                UserId = x.UserId,
                OtpHash = x.OtpHash,
                ExpiresAt = x.ExpiresAt,
                IsUsed = x.IsUsed,
                AttemptCount = x.AttemptCount
            })
            .FirstOrDefaultAsync();
    }

    public async Task IncrementAttemptAsync(long emailOtpId)
    {
        var otp = await _dbContext.EmailOtps
            .FirstOrDefaultAsync(x =>
                x.EmailOtpId == emailOtpId);

        if (otp == null)
            return;

        otp.AttemptCount++;

        await _dbContext.SaveChangesAsync();
    }

    public async Task MarkAsUsedAsync(long emailOtpId)
    {
        var otp = await _dbContext.EmailOtps
            .FirstOrDefaultAsync(x =>
                x.EmailOtpId == emailOtpId);

        if (otp == null)
            return;

        otp.IsUsed = true;
        otp.UsedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
    }
}
using EmployeeManagement.Application.DTOs.Auth;
using EmployeeManagement.Application.Interfaces.Repositories;
using EmployeeManagement.Domain.Constants;
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
            OtpStatus = OtpStatusConstants.Pending,
            AttemptCount = 0,
            CreatedAt = DateTime.Now
        };

        _dbContext.EmailOtps.Add(entity);

        await _dbContext.SaveChangesAsync();
    }

    public async Task<EmailOtpDataDto?> GetLatestUnusedAsync(int userId)
    {
        return await _dbContext.EmailOtps.AsNoTracking().Where(x => x.UserId == userId && x.OtpStatus == OtpStatusConstants.Pending)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new EmailOtpDataDto
            {
                EmailOtpId = x.EmailOtpId,
                UserId = x.UserId,
                OtpHash = x.OtpHash,
                ExpiresAt = x.ExpiresAt,
                IsUsed = OtpStatusConstants.Verified.Equals(x.OtpStatus),
                AttemptCount = x.AttemptCount
            }).FirstOrDefaultAsync();
    }

    public async Task IncrementAttemptAsync(long emailOtpId)
    {
        var otp = await _dbContext.EmailOtps.FirstOrDefaultAsync(x => x.EmailOtpId == emailOtpId);

        if (otp == null)
            return;

        otp.AttemptCount++;

        await _dbContext.SaveChangesAsync();
    }

    public async Task MarkAsUsedAsync(long emailOtpId)
    {
        var otp = await _dbContext.EmailOtps.FirstOrDefaultAsync(x => x.EmailOtpId == emailOtpId);

        if (otp == null)
            return;

        otp.OtpStatus = OtpStatusConstants.Verified;
        otp.VerifiedAt = DateTime.Now;

        await _dbContext.SaveChangesAsync();
    }

    public async Task InvalidateUnusedOtpsAsync(int userId)
    {
        var otps = await _dbContext.EmailOtps.Where(x => x.UserId == userId && x.OtpStatus == OtpStatusConstants.Pending).ToListAsync();

        if (otps.Count == 0)
            return;

        foreach (var otp in otps)
        {
            otp.OtpStatus = OtpStatusConstants.Revoked;
            otp.RevokedAt = DateTime.Now;
        }

        await _dbContext.SaveChangesAsync();
    }
}
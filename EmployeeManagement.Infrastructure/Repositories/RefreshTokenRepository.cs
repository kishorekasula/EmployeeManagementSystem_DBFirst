using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeManagement.Application.Interfaces.Repositories;
using EmployeeManagement.Infrastructure.Data;
using EmployeeManagement.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly EmployeeManagementDbContext _dbContext;

    public RefreshTokenRepository(EmployeeManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SaveAsync(int userId, string token, DateTime expiresAt)
    {
        var refreshToken = new RefreshToken
        {
            UserId = userId,
            Token = token,
            ExpiresAt = expiresAt,
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false
        };

        _dbContext.RefreshTokens.Add(refreshToken);

        await _dbContext.SaveChangesAsync();
    }

    public async Task RevokeAllAsync(int userId)
    {
        var tokens = await _dbContext.RefreshTokens.Where(x => x.UserId == userId && !x.IsRevoked).ToListAsync();

        foreach (var token in tokens)
        {
            token.IsRevoked = true;
        }

        await _dbContext.SaveChangesAsync();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Application.Interfaces.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task SaveAsync(int userId, string Token, DateTime expiresAt);

        Task RevokeAllAsync(int userId);
    }
}

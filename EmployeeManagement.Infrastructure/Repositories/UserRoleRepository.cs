using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeManagement.Application.DTOs.Users;
using EmployeeManagement.Application.Interfaces.Repositories;
using EmployeeManagement.Infrastructure.Data;
using EmployeeManagement.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Infrastructure.Repositories
{
    public class UserRoleRepository : IUserRoleRepository
    {
        private readonly EmployeeManagementDbContext _dbContext;
        public UserRoleRepository(EmployeeManagementDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<UserRoleResponseDto>> GetUserRolesAsync(int userId)
        {
            return await _dbContext.UserRoles
                .AsNoTracking().Where(ur => ur.UserId == userId)
                .Select(ur => new UserRoleResponseDto
                {
                    user_role_id = ur.UserRoleId,
                    user_id = ur.UserId,
                    role_id = ur.RoleId,
                    role_code = ur.Role.RoleCode,
                    role_name = ur.Role.RoleName,
                    assigned_at = ur.AssignedAt
                })
                .OrderBy(ur => ur.role_name)
                .ToListAsync();
        }

        public async Task<UserRoleResponseDto?> AssignRoleAsync(int userId, AssignRoleRequestDto request)
        {
            // Check whether user exists
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserId == userId);

            if(user == null)
            {
                throw new InvalidOperationException($"User with ID {userId} not found.");
            }

            // Check whether role exists
            var role = await _dbContext.Roles.FirstOrDefaultAsync(x => x.RoleId == request.role_id && x.IsActive);

            if(role == null)
            {
                throw new InvalidOperationException($"Active role with ID {request.role_id} not found.");
            }

            // Check whether user already has this role
            var existingUserRole = await _dbContext.UserRoles.FirstOrDefaultAsync(x => x.UserId == userId && x.RoleId == request.role_id);

            if(existingUserRole != null)
            {
                throw new InvalidOperationException("This role is already assigned to the user.");
            }

            var userRole = new UserRole
            {
                UserId = userId,
                RoleId = request.role_id,
                AssignedAt = DateTime.UtcNow
            };

            _dbContext.UserRoles.Add(userRole);

            await _dbContext.SaveChangesAsync();

            return new UserRoleResponseDto
            {
                user_role_id = userRole.UserRoleId,
                user_id = userRole.UserId,
                role_id = userRole.RoleId,
                role_code = userRole.Role.RoleCode,
                role_name = userRole.Role.RoleName,
                assigned_at = userRole.AssignedAt
            };
        }

        public async Task<bool> RemoveRoleAsync(int userId, int roleId)
        {
            var userRole = await _dbContext.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);

            if (userRole == null)
            {
                return false; // UserRole not found
            }

            _dbContext.UserRoles.Remove(userRole);

            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RoleAlreadyAssignedAsync(int userId, int roleId)
        {
            return await _dbContext.UserRoles
                .AsNoTracking()
                .AnyAsync(x =>
                    x.UserId == userId &&
                    x.RoleId == roleId);
        }

        public async Task<bool> HasRoleAsync(int userId, int roleId)
        {
            return await _dbContext.UserRoles
                .AsNoTracking()
                .AnyAsync(x =>
                    x.UserId == userId &&
                    x.RoleId == roleId);
        }

        public async Task<bool> UserExistsAsync(int userId)
        {
            return await _dbContext.Users
                .AsNoTracking()
                .AnyAsync(x => x.UserId == userId);
        }

        public async Task<bool> RoleExistsAsync(int roleId)
        {
            return await _dbContext.Roles
                .AsNoTracking()
                .AnyAsync(x => x.RoleId == roleId);
        }
    }
}

using EmployeeManagement.Application.Interfaces.Repositories;
using EmployeeManagement.Infrastructure.Data;
using EmployeeManagement.Application.DTOs.Roles;
using EmployeeManagement.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Infrastructure.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly EmployeeManagementDbContext _dbContext;

        public RoleRepository(EmployeeManagementDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<RoleResponseDto>> GetAllRolesAsync()
        {

            if (_dbContext == null)
            {
                throw new Exception("EmployeeManagementDbContext is NULL.");
            }

            if (_dbContext.Roles == null)
            {
                throw new Exception("Roles DbSet is NULL.");
            }

            return await _dbContext.Roles.Select(role => new RoleResponseDto
                {
                    role_id = role.RoleId,
                    role_code = role.RoleCode,
                    role_name = role.RoleName,
                    description = role.Description,
                    is_active = role.IsActive,
                    created_at = role.CreatedAt,
                    updated_at = role.UpdatedAt
                }).OrderBy(role => role.role_name)
                .ToListAsync();
        }

        public Task<RoleResponseDto?> GetRoleByIdAsync(int id)
        {
            return _dbContext.Roles.AsNoTracking()
                .Where(role => role.RoleId == id)
                .Select(role => new RoleResponseDto
                {
                    role_id = role.RoleId,
                    role_code = role.RoleCode,
                    role_name = role.RoleName,
                    description = role.Description,
                    is_active = role.IsActive,
                    created_at = role.CreatedAt,
                    updated_at = role.UpdatedAt
                })
                .FirstOrDefaultAsync();
        }

        public Task<bool> ExistsByCodeAsync(string roleCode)
        {
            roleCode = roleCode.Trim();
            return _dbContext.Roles.AsNoTracking().AnyAsync(role => role.RoleCode == roleCode);
        }

        public Task<bool> ExistsByNameAsync(string roleName)
        {
            roleName = roleName.Trim();
            return _dbContext.Roles.AsNoTracking().AnyAsync(role => role.RoleName == roleName);
        }

        public Task<RoleResponseDto> CreateRoleAsync(CreateRoleRequestDto request)
        {
            var roel = new Role
            {
                RoleCode = request.role_code.Trim(),
                RoleName = request.role_name.Trim(),
                Description = request.description?.Trim(),
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            _dbContext.Roles.Add(roel);

            return _dbContext.SaveChangesAsync()
                .ContinueWith(task =>
                {
                    if (task.IsCompletedSuccessfully)
                    {
                        return new RoleResponseDto
                        {
                            role_id = roel.RoleId,
                            role_code = roel.RoleCode,
                            role_name = roel.RoleName,
                            description = roel.Description,
                            is_active = roel.IsActive,
                            created_at = roel.CreatedAt,
                            updated_at = roel.UpdatedAt
                        };
                    }
                    else
                    {
                        throw task.Exception ?? new Exception("An error occurred while creating the role.");
                    }
                });
        }

        public async Task<RoleResponseDto> UpdateRoleAsync(int roleId, UpdateRoleRequestDto request)
        {
            var role = await _dbContext.Roles.FirstOrDefaultAsync(x => x.RoleId == roleId);

            if (role == null)
            {
                throw new InvalidOperationException($"Role with ID {roleId} not found.");
            }

            role.RoleCode = request.role_code.Trim().ToUpperInvariant();
            role.RoleName = request.role_name.Trim();
            role.Description = request.description?.Trim();
            role.IsActive = request.is_active;
            role.UpdatedAt = DateTime.Now;

            await _dbContext.SaveChangesAsync();

            return new RoleResponseDto
            {
                role_id = role.RoleId,
                role_code = role.RoleCode,
                role_name = role.RoleName,
                description = role.Description,
                is_active = role.IsActive,
                created_at = role.CreatedAt,
                updated_at = role.UpdatedAt
            };
        }

        public async Task<bool> DeleteRoleAsync(int roleId)
        {
            var role = await _dbContext.Roles.FirstOrDefaultAsync(x => x.RoleId == roleId);

            if (role == null)
            {
                return false;
            }

            // Soft delete
            role.IsActive = false;
            role.UpdatedAt = DateTime.Now;

            await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}

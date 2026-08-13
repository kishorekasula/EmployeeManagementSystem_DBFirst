using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeManagement.Application.Common.Exceptions;
using EmployeeManagement.Application.DTOs.Roles;
using EmployeeManagement.Application.Interfaces.Repositories;
using EmployeeManagement.Application.Interfaces.Services;

namespace EmployeeManagement.Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;

        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<List<RoleResponseDto>> GetAllRolesAsync()
        {
            return await _roleRepository.GetAllRolesAsync();
        }

        public async Task<RoleResponseDto?> GetRoleByIdAsync(int id)
        {
            var role = await _roleRepository.GetRoleByIdAsync(id);
            if (role == null)
            {
                throw new NotFoundException($"Role with ID {id} not found.");
            }
            return role;
        }

        public async Task<RoleResponseDto> CreateRoleAsync(CreateRoleRequestDto request)
        {
            var roleCode = request.role_code.Trim().ToUpperInvariant();
            var rolename = request.role_name.Trim();

            if(string.IsNullOrEmpty(roleCode))
            {
                throw new ArgumentException("Role code cannot be null or empty.", nameof(request.role_code));
            }

            if(string.IsNullOrEmpty(rolename))
            {
                throw new ArgumentException("Role name cannot be null or empty.", nameof(request.role_name));
            }

            if (await _roleRepository.ExistsByCodeAsync(roleCode))
            {
                throw new ConflictException($"Role with code '{roleCode}' already exists.");
            }

            if (await _roleRepository.ExistsByNameAsync(rolename))
            {
                throw new ConflictException($"Role with name '{rolename}' already exists.");
            }

            request.role_code = roleCode;
            request.role_name = rolename;

            return await _roleRepository.CreateRoleAsync(request);
        }

        public async Task<RoleResponseDto> UpdateRoleAsync(int roleId, UpdateRoleRequestDto request)
        {
            var existingRole = await _roleRepository.GetRoleByIdAsync(roleId);

            if (existingRole == null)
            {
                throw new NotFoundException($"Role with ID {roleId} not found.");
            }

            var roleCode = request.role_code.Trim().ToUpperInvariant();
            var rolename = request.role_name.Trim();

            if (string.IsNullOrEmpty(roleCode))
            {
                throw new ArgumentException("Role code cannot be null or empty.", nameof(request.role_code));
            }

            if (string.IsNullOrEmpty(rolename))
            {
                throw new ArgumentException("Role name cannot be null or empty.", nameof(request.role_name));
            }

            if(!string.Equals(existingRole.role_code, roleCode, StringComparison.OrdinalIgnoreCase))
            {
                if (await _roleRepository.ExistsByCodeAsync(roleCode))
                {
                    throw new ConflictException($"Role with code '{roleCode}' already exists.");
                }
            }

            if (!string.Equals(existingRole.role_name, rolename, StringComparison.OrdinalIgnoreCase))
            {
                if (await _roleRepository.ExistsByNameAsync(rolename))
                {
                    throw new ConflictException($"Role with name '{rolename}' already exists.");
                }
            }

            request.role_code = roleCode;
            request.role_name = rolename;
            return await _roleRepository.UpdateRoleAsync(roleId, request);
        }

        public async Task DeleteRoleAsync(int roleId)
        {
            var existingRole = await _roleRepository.GetRoleByIdAsync(roleId);
            if (existingRole == null)
            {
                throw new NotFoundException($"Role with ID {roleId} not found.");
            }

            await _roleRepository.DeleteRoleAsync(roleId);
        }
    }
}

using EmployeeManagement.Application.DTOs.Departments;
using EmployeeManagement.Application.Interfaces.Repositories;
using EmployeeManagement.Infrastructure.Data;
using EmployeeManagement.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Infrastructure.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly EmployeeManagementDbContext _dbContext;

        public DepartmentRepository(EmployeeManagementDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<DepartmentResponseDto>> GetAllDepartmentsAsync()
        {
            return await _dbContext.Departments.AsNoTracking().Select(d => new DepartmentResponseDto
            {
                department_id = d.DepartmentId,
                department_code = d.DepartmentCode,
                department_name = d.DepartmentName,
                department_description = d.DepartmentDescription,
                is_active = d.IsActive,
                created_at = d.CreatedAt,
                updated_at = d.UpdatedAt
            }).OrderBy(d => d.department_name).ToListAsync();
        }

        public async Task<DepartmentResponseDto?> GetDepartmentByIdAsync(int departmentId)
        {
            return await _dbContext.Departments.AsNoTracking().Where(d => d.DepartmentId == departmentId)
                .Select(d => new DepartmentResponseDto
                {
                    department_id = d.DepartmentId,
                    department_code = d.DepartmentCode,
                    department_name = d.DepartmentName,
                    department_description = d.DepartmentDescription,
                    is_active = d.IsActive,
                    created_at = d.CreatedAt,
                    updated_at = d.UpdatedAt
                }).FirstOrDefaultAsync();
        }

        public async Task<bool> ExistsDepartmentByCodeAsync(string departmentCode)
        {
            return await _dbContext.Departments.AnyAsync(d => d.DepartmentCode == departmentCode);
        }

        public async Task<bool> ExistsDepartmentByNameAsync(string departmentName)
        {
            return await _dbContext.Departments.AnyAsync(d => d.DepartmentName == departmentName);
        }

        public async Task<DepartmentResponseDto> CreateDepartmentAsync(CreateDepartmentRequestDto createDepartmentRequestDto)
        {
            var department = new Department
            {
                DepartmentCode = createDepartmentRequestDto.department_code,
                DepartmentName = createDepartmentRequestDto.department_name,
                DepartmentDescription = createDepartmentRequestDto.department_description,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            _dbContext.Departments.Add(department);

            await _dbContext.SaveChangesAsync();

            return new DepartmentResponseDto
            {
                department_id = department.DepartmentId,
                department_code = department.DepartmentCode,
                department_name = department.DepartmentName,
                department_description = department.DepartmentDescription,
                is_active = department.IsActive,
                created_at = department.CreatedAt,
                updated_at = department.UpdatedAt
            };
        }

        public async Task<DepartmentResponseDto> UpdateDepartmentAsync(int departmentId, UpdateDepartmentRequestDto updateDepartmentRequestDto)
        {
            var department = await _dbContext.Departments.FirstOrDefaultAsync(d => d.DepartmentId == departmentId);

            if (department == null)
            {
                throw new InvalidOperationException("Department not found");
            }

            try
            {
                department.DepartmentCode = updateDepartmentRequestDto.department_code;
                department.DepartmentName = updateDepartmentRequestDto.department_name;
                department.DepartmentDescription = updateDepartmentRequestDto.department_description;
                department.IsActive = updateDepartmentRequestDto.is_active;
                department.UpdatedAt = DateTime.Now;

                await _dbContext.SaveChangesAsync();
            }

            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error updating department: {ex.Message}", ex);
            }

            return new DepartmentResponseDto
            {
                department_id = department.DepartmentId,
                department_code = department.DepartmentCode,
                department_name = department.DepartmentName,
                department_description = department.DepartmentDescription,
                is_active = department.IsActive,
                created_at = department.CreatedAt,
                updated_at = department.UpdatedAt
            };
        }

        public async Task<bool> DeleteDepartmentAsync(int departmentId)
        {
            return await _dbContext.Departments.Where(d => d.DepartmentId == departmentId).ExecuteDeleteAsync() > 0;
        }
    }
}

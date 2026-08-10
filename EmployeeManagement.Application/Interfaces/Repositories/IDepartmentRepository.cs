using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeManagement.Application.DTOs.Departments;

namespace EmployeeManagement.Application.Interfaces.Repositories
{
    public interface IDepartmentRepository
    {
        Task<List<DepartmentResponseDto>> GetAllDepartmentsAsync();
        Task<DepartmentResponseDto> GetDepartmentByIdAsync(int departmentId);
        Task<bool> ExistsDepartmentByCodeAsync(string departmentCode);
        Task<bool> ExistsDepartmentByNameAsync(string departmentName);
        Task<DepartmentResponseDto> CreateDepartmentAsync(CreateDepartmentRequestDto createDepartmentRequestDto);
        Task<DepartmentResponseDto> UpdateDepartmentAsync(int departmentId, UpdateDepartmentRequestDto updateDepartmentRequestDto);
        Task<bool> DeleteDepartmentAsync(int departmentId);
    }
}

using EmployeeManagement.Application.DTOs.Departments;
using EmployeeManagement.Application.Interfaces.Repositories;
using EmployeeManagement.Application.Interfaces.Services;


namespace EmployeeManagement.Application.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;

        public DepartmentService(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        public async Task<List<DepartmentResponseDto>> GetAllDepartmentsAsync()
        {
            return await _departmentRepository.GetAllDepartmentsAsync();
        }

        public async Task<DepartmentResponseDto> CreateDepartmentAsync(CreateDepartmentRequestDto createDepartmentRequestDto)
        {
            // Check if department code already exists
            if (await _departmentRepository.ExistsDepartmentByCodeAsync(createDepartmentRequestDto.department_code))
            {
                throw new Exception($"Department with code '{createDepartmentRequestDto.department_code}' already exists.");
            }
            // Check if department name already exists
            if (await _departmentRepository.ExistsDepartmentByNameAsync(createDepartmentRequestDto.department_name))
            {
                throw new Exception($"Department with name '{createDepartmentRequestDto.department_name}' already exists.");
            }
            return await _departmentRepository.CreateDepartmentAsync(createDepartmentRequestDto);
        }

        public async Task<DepartmentResponseDto?> GetDepartmentByIdAsync(int departmentId)
        {
            return await _departmentRepository.GetDepartmentByIdAsync(departmentId);
        }
    }
}

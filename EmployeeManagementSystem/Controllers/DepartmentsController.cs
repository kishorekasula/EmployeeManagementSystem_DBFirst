using EmployeeManagement.Application.DTOs.Departments;
using EmployeeManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DepartmentsController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;
        public DepartmentsController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [Authorize(Roles = "Admin,Employee")]
        [HttpGet]
        public async Task<IActionResult> GetAllDepartments()
        {
            var departments = await _departmentService.GetAllDepartmentsAsync();
            return Ok(new
            {
                statusCode = StatusCodes.Status200OK,
                data = departments
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateDepartment([FromBody] CreateDepartmentRequestDto createDepartmentRequestDto)
        {
            try
            {
                var department = await _departmentService.CreateDepartmentAsync(createDepartmentRequestDto);
                return StatusCode(StatusCodes.Status201Created, new
                {
                    statusCode = StatusCodes.Status201Created,
                    message = "Department created successfully.",
                    data = department
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new
                {
                    statusCode = StatusCodes.Status400BadRequest,
                    message = ex.Message
                });
            }
        }

        [HttpGet("{departmentId}")]
        public async Task<IActionResult> GetDepartmentById(int departmentId)
        {
            var department = await _departmentService.GetDepartmentByIdAsync(departmentId);
            if (department == null)
            {
                return NotFound(new
                {
                    statusCode = StatusCodes.Status404NotFound,
                    message = "Department not found."
                });
            }
            return Ok(new
            {
                statusCode = StatusCodes.Status200OK,
                data = department
            });
        }

        [HttpPut("{departmentId}")]
        public async Task<IActionResult> UpdateDepartment(int departmentId, [FromBody] UpdateDepartmentRequestDto updateDepartmentRequestDto)
        {
            try
            {
                var department = await _departmentService.UpdateDepartmentAsync(departmentId, updateDepartmentRequestDto);

                if (department == null)
                {
                    return NotFound(new
                    {
                        statusCode = StatusCodes.Status404NotFound,
                        message = "Department not found."
                    });
                }

                return Ok(new
                {
                    statusCode = StatusCodes.Status200OK,
                    message = "Department updated successfully.",
                    data = department
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new
                {
                    statusCode = StatusCodes.Status400BadRequest,
                    message = ex.Message
                });
            }
        }

        [HttpDelete("{departmentId}")]
        public async Task<IActionResult> DeleteDepartment(int departmentId)
        {
            try
            {
                var result = await _departmentService.DeleteDepartmentAsync(departmentId);
                if (!result)
                {
                    return NotFound(new
                    {
                        statusCode = StatusCodes.Status404NotFound,
                        message = "Department not found."
                    });
                }
                return Ok(new
                {
                    statusCode = StatusCodes.Status200OK,
                    message = "Department deleted successfully."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new
                {
                    statusCode = StatusCodes.Status400BadRequest,
                    message = ex.Message
                });
            }
        }
    }
}

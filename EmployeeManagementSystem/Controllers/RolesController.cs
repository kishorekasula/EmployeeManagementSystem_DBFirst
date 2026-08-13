using Microsoft.AspNetCore.Mvc;
using EmployeeManagement.Application.Interfaces.Services;
using EmployeeManagement.Application.DTOs.Roles;
using Microsoft.AspNetCore.Authorization;


namespace EmployeeManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet("GetAllRoles")]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _roleService.GetAllRolesAsync();
            return Ok(new
            {
                statusCode = StatusCodes.Status200OK,
                data = roles
            });
        }

        [HttpGet("GetRoleById/{roleId}")]
        public async Task<IActionResult> GetRoleById([FromRoute] int roleId)
        {
            var role = await _roleService.GetRoleByIdAsync(roleId);

            if (role == null)
            {
                return NotFound(new
                {
                    statusCode = StatusCodes.Status404NotFound,
                    message = "Role not found."
                });
            }

            return Ok(new
            {
                statusCode = StatusCodes.Status200OK,
                data = role
            });
        }

        [HttpPost("CreateRole")]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequestDto createRoleRequestDto)
        {
            try
            {
                var role = await _roleService.CreateRoleAsync(createRoleRequestDto);
                return StatusCode(StatusCodes.Status201Created, new
                {
                    statusCode = StatusCodes.Status201Created,
                    message = "Role created successfully.",
                    data = role
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

        [HttpPut("UpdateRole/{roleId}")]
        public async Task<IActionResult> UpdateRole([FromRoute] int roleId, [FromBody] UpdateRoleRequestDto updateRoleRequestDto)
        {
            try
            {
                var role = await _roleService.UpdateRoleAsync(roleId, updateRoleRequestDto);
                return Ok(new
                {
                    statusCode = StatusCodes.Status200OK,
                    message = "Role updated successfully.",
                    data = role
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

        [HttpDelete("DeleteRole/{roleId}")]
        public async Task<IActionResult> DeleteRole([FromRoute] int roleId)
        {
            try
            {
                await _roleService.DeleteRoleAsync(roleId);
                return Ok(new
                {
                    statusCode = StatusCodes.Status200OK,
                    message = "Role deleted successfully."
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

using EmployeeManagement.Application.Common.Exceptions;
using EmployeeManagement.Application.DTOs.Users;
using EmployeeManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace EmployeeManagementSystem.API.Controllers;

[ApiController]
[Route("api/Users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IUserRoleService _userRoleService;

    public UsersController(IUserService userService, IUserRoleService userRoleService)
    {
        _userService = userService;
        _userRoleService = userRoleService;
    }

    [HttpGet("GetAllUsers")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetAllAsync();

        return StatusCode(StatusCodes.Status200OK,
            new
            {
                statusCode = StatusCodes.Status200OK,
                data = users
            });
    }

    [HttpGet("GetUserById/{userId}")]
    public async Task<IActionResult> GetUserById(int userId)
    {
        var user = await _userService.GetByIdAsync(userId);

        if (user == null)
        {
            return StatusCode(StatusCodes.Status404NotFound,
                new
                {
                    statusCode = StatusCodes.Status404NotFound,
                    message = "User not found."
                });
        }

        return StatusCode(StatusCodes.Status200OK,
            new
            {
                statusCode = StatusCodes.Status200OK,
                data = user
            });
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("CreateUser")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequestDto request)
    {
        try
        {
            var createdUser = await _userService.CreateAsync(request);
            return StatusCode(StatusCodes.Status201Created,
                new
                {
                    statusCode = StatusCodes.Status201Created,
                    message = "User created successfully.",
                    data = createdUser
                });
        }
        catch (ConflictException ex)
        {
            return Conflict(
                new
                {
                    statusCode = StatusCodes.Status409Conflict,
                    message = ex.Message
                });
        }
        catch (Exception ex)
        {
            return BadRequest(
                new
                {
                    statusCode = StatusCodes.Status400BadRequest,
                    details = ex.Message
                });
        }
    }

    [HttpPut("UpdateUser/{userId}")]
    public async Task<IActionResult> UpdateUser(int userId, [FromBody] UpdateUserRequestDto request)
    {
        try
        {
            var user = await _userService.UpdateAsync(userId, request);

            if (user == null)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                    new
                    {
                        statusCode = StatusCodes.Status404NotFound,
                        message = "User not found."
                    });
            }

            return Ok(new
            {
                statusCode = StatusCodes.Status200OK,
                message = "User updated successfully.",
                data = user
            });
        }

        catch (Exception ex)
        {
            return BadRequest(
                new
                {
                    statusCode = StatusCodes.Status400BadRequest,
                    details = ex.Message
                });
        }
    }

    [HttpDelete("DeleteUser/{userId}")]
    public async Task<IActionResult> DeleteUser(int userId)
    {
        try
        {
            var result = await _userService.DeleteAsync(userId);

            if (!result)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                    new
                    {
                        statusCode = StatusCodes.Status404NotFound,
                        message = "User not found."
                    });
            }
            return Ok(new
            {
                statusCode = StatusCodes.Status200OK,
                message = "User deleted successfully."
            });
        }
        catch (Exception ex)
        {
            return BadRequest(
                new
                {
                    statusCode = StatusCodes.Status400BadRequest,
                    details = ex.Message
                });
        }
    }

    [HttpGet("GetUserRoles/{userId}")]
    public async Task<IActionResult> GetUserRoles(int userId)
    {
        try
        {
            var roles = await _userRoleService.GetUserRolesAsync(userId);
            return Ok(new
            {
                statusCode = StatusCodes.Status200OK,
                data = roles
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(
                new
                {
                    statusCode = StatusCodes.Status404NotFound,
                    message = ex.Message
                });
        }
        catch (Exception ex)
        {
            return BadRequest(
                new
                {
                    statusCode = StatusCodes.Status400BadRequest,
                    details = ex.Message
                });
        }
    }

    [HttpPost("AssignRole/{userId}")]
    public async Task<IActionResult> AssignRole(int userId, [FromBody] AssignRoleRequestDto request)
    {
        try
        {
            var result = await _userRoleService.AssignRoleAsync(userId, request);

            return Ok(new
            {
                statusCode = StatusCodes.Status200OK,
                message = "Role assigned successfully.",
                data = result
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                statusCode = StatusCodes.Status404NotFound,
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                statusCode = StatusCodes.Status400BadRequest,
                message = ex.Message
            });
        }
    }

    [HttpDelete("RemoveRole/{userId}/{roleId}")]
    public async Task<IActionResult> RemoveRole(int userId, int roleId)
    {
        try
        {
            await _userRoleService.RemoveRoleAsync(userId, roleId);

            return Ok(new
            {
                statusCode = StatusCodes.Status200OK,
                message = "Role removed successfully."
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                statusCode = StatusCodes.Status404NotFound,
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                statusCode = StatusCodes.Status400BadRequest,
                message = ex.Message
            });
        }
    }
}
using EmployeeManagement.Application.Interfaces.Services;
using EmployeeManagement.Application.Common.Exceptions;
using EmployeeManagement.Application.DTOs.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userService.GetAllAsync();

        return StatusCode( StatusCodes.Status200OK,
            new
            {
                statusCode = StatusCodes.Status200OK,
                data = users
            });
    }

    [HttpGet("{userId:int}")]
    public async Task<IActionResult> GetById(int userId)
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
}
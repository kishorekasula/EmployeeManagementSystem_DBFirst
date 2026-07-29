using EmployeeManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
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

        return StatusCode(
            StatusCodes.Status200OK,
            new
            {
                statusCode = StatusCodes.Status200OK,
                data = user
            });
    }
}
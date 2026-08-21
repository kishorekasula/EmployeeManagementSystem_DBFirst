using EmployeeManagement.Application.Common.Exceptions;
using EmployeeManagement.Application.DTOs.Auth;
using EmployeeManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static System.Net.WebRequestMethods;

namespace EmployeeManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        try
        {
            var result = await _authService.RegisterAsync(request);

            return StatusCode(StatusCodes.Status201Created,
                new
                {
                    statusCode = StatusCodes.Status201Created,

                    message = "User registered successfully.",

                    data = result
                });
        }
        catch (ConflictException ex)
        {
            return StatusCode(
                StatusCodes.Status409Conflict,
                new
                {
                    statusCode = StatusCodes.Status409Conflict,

                    message = ex.Message
                });
        }
    }

    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail( [FromBody] VerifyEmailRequestDto request)
    {
        try
        {
            await _authService.VerifyEmailAsync(request);

            return StatusCode( StatusCodes.Status200OK,
                new
                {
                    statusCode = StatusCodes.Status200OK,

                    message = "Email verified successfully."
                });
        }
        catch (NotFoundException ex)
        {
            return StatusCode(StatusCodes.Status404NotFound,
                new
                {
                    statusCode = StatusCodes.Status404NotFound,

                    message = ex.Message
                });
        }
        catch (BadRequestException ex)
        {
            return StatusCode( StatusCodes.Status400BadRequest,
                new
                {
                    statusCode = StatusCodes.Status400BadRequest,

                    message = ex.Message
                });
        }
    }

    [HttpPost("resend-otp")]
    public async Task<IActionResult> ResendOtp([FromBody] ResendOtpRequestDto request)
    {
        try
        {
            await _authService.ResendOtpAsync(request);

            return StatusCode(StatusCodes.Status200OK,
                new
                {
                    statusCode = StatusCodes.Status200OK,

                    message = "A new verification OTP has been sent."
                });
        }
        catch (NotFoundException ex)
        {
            return StatusCode(StatusCodes.Status404NotFound,
                new
                {
                    statusCode = StatusCodes.Status404NotFound,
                    message = ex.Message
                });
        }
        catch (BadRequestException ex)
        {
            return StatusCode(StatusCodes.Status400BadRequest,
                new
                {
                    statusCode = StatusCodes.Status400BadRequest,
                    message = ex.Message
                });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
    LoginRequestDto request)
    {
        var result = await _authService.LoginAsync(request);

        return Ok(new
        {
            statusCode = StatusCodes.Status200OK,
            message = "Login successful.",
            data = result
        });
    }

    [Authorize]
    [HttpPost("Logout")]
    public IActionResult Logout()
    {
        // Implement logout logic here, such as clearing cookies or tokens
        return Ok(new
        {
            statusCode = StatusCodes.Status200OK,
            message = "Logout successful."
        });
    }
}
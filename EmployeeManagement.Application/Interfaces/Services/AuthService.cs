using EmployeeManagement.Application.Common.Exceptions;
using EmployeeManagement.Application.DTOs.Auth;
using EmployeeManagement.Application.DTOs.Users;
using EmployeeManagement.Application.Interfaces.Repositories;
using EmployeeManagement.Application.Interfaces.Services;

namespace EmployeeManagement.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IOtpService _otpService;
    private readonly IEmailOtpRepository _emailOtpRepository;
    private readonly IEmailService _emailService;

    public AuthService(
          IUserRepository userRepository,
          IPasswordHasher passwordHasher,
          IOtpService otpService,
          IEmailOtpRepository emailOtpRepository,
          IEmailService emailService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _otpService = otpService;
        _emailOtpRepository = emailOtpRepository;
        _emailService = emailService;
    }

    public async Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var emailExists = await _userRepository.EmailExistsAsync(email);

        if (emailExists)
        {
            throw new ConflictException("An account with this email already exists.");
        }

        var passwordHash = _passwordHasher.Hash(request.Password);

        var userId = await _userRepository.CreateAsync(
            new CreateUserDataDto
            {
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                Email = email,
                PasswordHash = passwordHash
            },
            "EMPLOYEE");

        // Generate OTP
        var otp = _otpService.GenerateOtp();

        // Never store plain OTP
        var otpHash = _otpService.HashOtp(otp);

        // Expiry comes from configuration
        var expiresAt = _otpService.GetExpiryTime();

        // Store OTP hash
        await _emailOtpRepository.CreateAsync(userId, otpHash, expiresAt);

        // Send actual OTP
        await _emailService.SendOtpAsync(email, request.FirstName.Trim(), otp);

        return new RegisterResponseDto
        {
            user_id = userId,
            first_name = request.FirstName.Trim(),
            last_name = request.LastName.Trim(),
            email = email,
            IsEmailVerified = false,
            roles = new List<string>
            {
                "Employee"
            }
        };
    }

    public async Task VerifyEmailAsync(VerifyEmailRequestDto request)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        // 1. Find user
        var user = await _userRepository.GetByEmailAsync(email);

        if (user == null)
        {
            throw new NotFoundException("User not found.");
        }

        // 2. Already verified?
        if (user.IsEmailVerified)
        {
            throw new BadRequestException("Email is already verified.");
        }

        // 3. Get latest unused OTP
        var otpRecord = await _emailOtpRepository.GetLatestUnusedAsync(user.UserId);

        if (otpRecord == null)
        {
            throw new BadRequestException("No active OTP was found. Please request a new OTP.");
        }

        // 4. Check expiration
        if (DateTime.UtcNow > otpRecord.ExpiresAt)
        {
            throw new BadRequestException("OTP has expired. Please request a new OTP.");
        }

        // 5. Check attempt limit
        var maxAttempts = _otpService.GetMaxAttempts();

        if (otpRecord.AttemptCount >= maxAttempts)
        {
            throw new BadRequestException("Maximum OTP verification attempts exceeded.");
        }

        // 6. Verify OTP
        var isValid = _otpService.VerifyOtp(request.Otp, otpRecord.OtpHash);

        if (!isValid)
        {
            await _emailOtpRepository.IncrementAttemptAsync(otpRecord.EmailOtpId);

            throw new BadRequestException("Invalid OTP.");
        }

        // 7. OTP is valid
        await _emailOtpRepository.MarkAsUsedAsync(otpRecord.EmailOtpId);

        // 8. Verify user email
        await _userRepository.MarkEmailAsVerifiedAsync(user.UserId);
    }
}
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
    private readonly IJwtService _jwtService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public AuthService(
          IUserRepository userRepository,
          IPasswordHasher passwordHasher,
          IOtpService otpService,
          IEmailOtpRepository emailOtpRepository,
          IEmailService emailService,
          IJwtService jwtService,
          IRefreshTokenRepository refreshTokenRepository)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _otpService = otpService;
        _emailOtpRepository = emailOtpRepository;
        _emailService = emailService;
        _jwtService = jwtService;
        _refreshTokenRepository = refreshTokenRepository;
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
        if (DateTime.Now > otpRecord.ExpiresAt)
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

        if (isValid)
        {
            _emailService.SendOnboardingEmailAsync(user.Email, user.FirstName, user.LastName);
        }

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

    public async Task ResendOtpAsync(ResendOtpRequestDto request)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        // 1. Find user
        var user = await _userRepository.GetByEmailAsync(email);

        if(user == null) 
        {
            throw new NotFoundException("User not found.");
        }

        // 2. Already verified?
        if(user.IsEmailVerified)
        {
            throw new BadRequestException("Email is already verified.");
        }

        // 3. Invalidate previous OTPs
        await _emailOtpRepository.InvalidateUnusedOtpsAsync(user.UserId);

        // 4. Generate new OTP
        var otp = _otpService.GenerateOtp();

        // 5. Hash OTP
        var otpHash = _otpService.HashOtp(otp);

        // 6. Calculate expiry
        var expiresAt = _otpService.GetExpiryTime();

        // 7. Save new OTP
        await _emailOtpRepository.CreateAsync(user.UserId, otpHash, expiresAt);

        // 8. Send actual OTP
        await _emailService.SendOtpAsync(user.Email, user.FirstName, otp);
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await _userRepository.GetLoginUserAsync(request.Email);
        
        if (user == null)
        {
            throw new NotFoundException("Invalid email or password.");
        }

        if(!user.IsEmailVerified)
        {
            throw new BadRequestException("Please verify your email before logging in.");
        }

        if(!user.IsActive)
        {
            throw new BadRequestException("Your account is inactive.");
        }

        var passwordValid = _passwordHasher.Verify(request.Password, user.PasswordHash);

        if (!passwordValid)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        var accessToken = _jwtService.GetAccessToken(
            new JwtUserDto
            {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            },
            user.Roles);

        var refreshToken = _jwtService.GetRefreshToken();

        await _refreshTokenRepository.RevokeAllAsync(user.UserId);

        await _refreshTokenRepository.SaveAsync(user.UserId, refreshToken, _jwtService.GetRefreshTokenExpiry());

        await _userRepository.UpdateLastLoginAsync(user.UserId);

        // Here you would typically validate the password and generate tokens
        // For now, we'll just return a basic login response
        return new LoginResponseDto
        {
            user_id = user.UserId,
            first_name = user.FirstName,
            last_name = user.LastName,
            email = user.Email,
            role = user.Roles,
            token = accessToken,

            //RefreshToken = refreshToken,

            //AccessTokenExpiry = _jwtService.GetAccessTokenExpiry(),

            //RefreshTokenExpiry = _jwtService.GetRefreshTokenExpiry()
        };
    }
}
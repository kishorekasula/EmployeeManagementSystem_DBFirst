using EmployeeManagement.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace EmployeeManagement.Infrastructure.Security;

public class OtpService : IOtpService
{
    private readonly IConfiguration _configuration;

    public OtpService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateOtp()
    {
        return RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
    }

    public string HashOtp(string otp)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(otp));

        return Convert.ToHexString(bytes);
    }

    public bool VerifyOtp(string otp, string otpHash)
    {
        var generatedHash = HashOtp(otp);

        return CryptographicOperations.FixedTimeEquals(Convert.FromHexString(generatedHash), Convert.FromHexString(otpHash));
    }

    public DateTime GetExpiryTime()
    {
        var expiryMinutes = _configuration.GetValue<int>("OtpSettings:ExpiryMinutes");

        //return DateTime.UtcNow.AddMinutes(expiryMinutes);
        return DateTime.Now.AddMinutes(expiryMinutes);
    }

    public int GetMaxAttempts()
    {
        return _configuration.GetValue<int>("OtpSettings:MaxAttempts");
    }
}
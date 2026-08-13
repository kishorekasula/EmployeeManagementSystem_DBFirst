using System.Text;
using EmployeeManagement.Application.Interfaces.Services;
using Microsoft.Extensions.Options;
using EmployeeManagement.Infrastructure.Configuration;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;

namespace EmployeeManagement.Infrastructure.Security
{
    public class JwtService : IJwtService
    {
        private readonly JwtSettings _jwtSettings;

        public JwtService(IOptions<JwtSettings> options)
        {
            _jwtSettings = options.Value;
        }

        public string GetAccessToken(JwtUserDto user, List<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Email, user.Email)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_jwtSettings.AccessTokenExpiryMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GetRefreshToken()
        {
            var randomBytes = new byte[64];

            using var rng = RandomNumberGenerator.Create();
   
            rng.GetBytes(randomBytes);

            return Convert.ToBase64String(randomBytes);
        }

        public DateTime GetAccessTokenExpiry()
        {
            return DateTime.Now.AddMinutes(_jwtSettings.AccessTokenExpiryMinutes);
        }

        public DateTime GetRefreshTokenExpiry()
        {
            return DateTime.Now.AddDays(_jwtSettings.RefreshTokenExpiryDays);
        }
    }
}

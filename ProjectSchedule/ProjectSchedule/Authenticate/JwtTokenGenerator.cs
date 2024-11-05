using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ProjectSchedule.Models;

namespace ProjectSchedule.Authenticate
{
    public class JwtTokenGenerator
    {
        private readonly JwtSetting _jwtSettings;

        public JwtTokenGenerator(JwtSetting jwtSettings)
        {
            _jwtSettings = jwtSettings;
        }

        public string GenerateJwtToken(Teacher teacher)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
              {
        new Claim(ClaimTypes.NameIdentifier, teacher.TeacherId.ToString()),
        new Claim(ClaimTypes.Name, teacher.Email),
        new Claim(ClaimTypes.Role, teacher.Role),
                  // Add custom claims as needed
              }),
                Expires = DateTime.UtcNow.AddHours(_jwtSettings.ExpiryHours),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
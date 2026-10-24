using HotelApp.Application.Contracts;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HotelApp.Infrastructure.Identity.Service
{
    internal class TokenService : ITokenService
    {
        private readonly JwtSettings _jwtSettings;
        public TokenService(IOptions<JwtSettings> jwsOptions)
        {
            _jwtSettings = jwsOptions.Value;
        }


        public string CreateToken(string userId, string email, string userName, IReadOnlyList<string> roles)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Name, userName)
            };

            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var secretKey = _jwtSettings.SecretKey;
            if (string.IsNullOrEmpty(secretKey))
                throw new InvalidOperationException("Jwt Secre Key Is Missing");

            if (secretKey.Length < 32)
                throw new InvalidOperationException("Jwt Secre Key Is Too Short");


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
                signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);

        }
    }


    public class JwtSettings
    {
        public string SecretKey { get; set; } = default!;
        public string Issuer { get; set; } = default!;
        public string Audience { get; set; } = default!;
        public int ExpirationMinutes { get; set; }
    }
}


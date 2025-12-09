using CitiesManager.Core.DTO;
using CitiesManager.Core.Identity;
using CitiesManager.Core.ServiceContracts;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CitiesManager.Core.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public AuthenticationResponse CreateJwtToken(ApplicationUser user)
        {
            DateTime expiration =
            DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:EXPIRATION_MINUTES"]));

            Claim[] claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat,DateTime.UtcNow.ToString()),
                new Claim(ClaimTypes.Name,user.PersonName),
                new Claim(ClaimTypes.NameIdentifier,user.Email),
                new Claim(ClaimTypes.Email,user.Email)
            };

            SymmetricSecurityKey securitykey = new(
                Encoding.UTF8.GetBytes(_configuration["Jwt:KEY"]));

            SigningCredentials signinCredentials = new(securitykey, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken tokenGenerator = new(
                issuer: _configuration["Jwt:ISSUER"],
                audience: _configuration["Jwt:AUDIENCE"],
                claims: claims,
                expires: expiration,
                signingCredentials: signinCredentials
                );

            string tokenAsString = new JwtSecurityTokenHandler().WriteToken(tokenGenerator);

            return new AuthenticationResponse
            {
                PersonName = user.PersonName!,
                Email = user.Email!,
                Token = tokenAsString,
                Expiration = expiration,
                RefreshToken = GenerateRefreshToken(),
                RefreshTokenExpiration = DateTime.UtcNow.AddMinutes(Convert.ToInt32(_configuration["EXPIRATION_MINUTES"]))
            };
        }

        private static string GenerateRefreshToken()
        {
            byte[] randomNumber = new byte[64];
            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public ClaimsPrincipal? GetPrincinpalFromJwtToken(string? token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration["Jwt:ISSUER"],
                ValidAudience = _configuration["Jwt:AUDIENCE"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:KEY"])),
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            ClaimsPrincipal principal =
                tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }
    }
}

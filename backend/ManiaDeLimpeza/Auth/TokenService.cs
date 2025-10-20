using ManiaDeLimpeza.Application.Interfaces;
using ManiaDeLimpeza.Infrastructure.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ManiaDeLimpeza.Api.Auth
{
    public class TokenService : IScopedDependency, ITokenService
    {
        private readonly AuthOptions _options;

        public TokenService(IOptions<AuthOptions> options)
        {
            _options = options.Value;
        }

        public string GenerateToken(string userId, string email)
        {
            var key = Encoding.ASCII.GetBytes(_options.JwtSecret);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Email, email),
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddSeconds(_options.ExpireTimeInSeconds),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}

using Microsoft.IdentityModel.Tokens;
using RutaSegura.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace RutaSegura.Services
{
    public class JwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public (string Token, string Jti, DateTime ExpiraEn) GenerateToken(Usuario usuario)
        {
            var rawKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT key is missing.");
            var issuer = _configuration["Jwt:Issuer"] ?? "RutaSegura.API";
            var audience = _configuration["Jwt:Audience"] ?? "RutaSegura.Client";
            var expires = DateTime.UtcNow.AddHours(8);
            var jti = Guid.NewGuid().ToString("N");
            var rol = string.IsNullOrWhiteSpace(usuario.Rol) ? "Usuario" : usuario.Rol;

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
                new(JwtRegisteredClaimNames.Email, usuario.Email),
                new(JwtRegisteredClaimNames.Jti, jti),
                new(ClaimTypes.Role, rol),
                new("nombre", usuario.Nombre ?? string.Empty),
            };

            var signingKey = JwtSigningKey.CreateFromSecret(rawKey);
            var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expires,
                signingCredentials: creds);

            return (new JwtSecurityTokenHandler().WriteToken(token), jti, expires);
        }
    }
}

using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace RutaSegura.Services
{
    /// <summary>
    /// HS256 exige al menos 256 bits; muchos despliegues (p. ej. variables auto-generadas)
    /// devuelven cadenas cortas. Derivamos 32 bytes con SHA-256 del texto configurado
    /// (igual en emisión y validación).
    /// </summary>
    public static class JwtSigningKey
    {
        public static SymmetricSecurityKey CreateFromSecret(string? rawSecret)
        {
            if (string.IsNullOrEmpty(rawSecret))
            {
                throw new InvalidOperationException("JWT key is missing.");
            }

            var keyBytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawSecret));
            return new SymmetricSecurityKey(keyBytes);
        }
    }
}

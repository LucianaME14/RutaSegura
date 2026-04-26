using System;
using System.Security.Cryptography;

namespace RutaSegura.Services
{
    public static class PasswordService
    {
        private const int Iterations = 100_000;
        private const int SaltSize = 16;
        private const int HashSize = 32;

        public static string Hash(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("La contraseña no puede estar vacía.", nameof(password));
            }

            using var rng = RandomNumberGenerator.Create();
            var salt = new byte[SaltSize];
            rng.GetBytes(salt);

            using var deriveBytes = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            var hash = deriveBytes.GetBytes(HashSize);

            return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }

        public static bool Verify(string storedValue, string providedPassword)
        {
            if (string.IsNullOrEmpty(storedValue) || string.IsNullOrEmpty(providedPassword))
            {
                return false;
            }

            try
            {
                var parts = storedValue.Split('.');
                if (parts.Length != 3)
                {
                    return string.Equals(storedValue, providedPassword, StringComparison.Ordinal);
                }

                if (!int.TryParse(parts[0], out var iterations) || iterations < 1)
                {
                    return false;
                }

                var salt = Convert.FromBase64String(parts[1]);
                var storedHash = Convert.FromBase64String(parts[2]);

                using var deriveBytes = new Rfc2898DeriveBytes(providedPassword, salt, iterations, HashAlgorithmName.SHA256);
                var computedHash = deriveBytes.GetBytes(storedHash.Length);

                return CryptographicOperations.FixedTimeEquals(storedHash, computedHash);
            }
            catch
            {
                // Hash en BD con formato inesperado: no desencadenar 500, solo login fallido
                return false;
            }
        }
    }
}

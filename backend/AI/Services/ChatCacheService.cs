using System.Security.Cryptography;
using System.Text;
using RutaSegura.Services;

namespace RutaSegura.AI.Services;

public interface IChatCacheService
{
    Task<string?> GetAsync(int userId, string message, CancellationToken ct = default);
    Task SetAsync(int userId, string message, string answer, CancellationToken ct = default);
}

/// <summary>Caché Redis de respuestas frecuentes del chatbot.</summary>
public class ChatCacheService : IChatCacheService
{
    private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(10);
    private readonly RedisService _redis;

    public ChatCacheService(RedisService redis) => _redis = redis;

    public async Task<string?> GetAsync(int userId, string message, CancellationToken ct = default)
    {
        if (!_redis.IsEnabled) return null;
        return await _redis.GetStringAsync(BuildKey(userId, message));
    }

    public async Task SetAsync(int userId, string message, string answer, CancellationToken ct = default)
    {
        if (!_redis.IsEnabled || string.IsNullOrWhiteSpace(answer)) return;
        await _redis.SetStringAsync(BuildKey(userId, message), answer, Ttl);
    }

    private static string BuildKey(int userId, string message)
    {
        var norm = message.Trim().ToLowerInvariant();
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(norm)))[..16];
        return $"safebot:chat:v1:{userId}:{hash}";
    }
}

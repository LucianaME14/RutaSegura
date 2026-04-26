using StackExchange.Redis;

namespace RutaSegura.Services
{
    public class RedisService
    {
        private readonly IDatabase? _database;
        private readonly bool _enabled;

        public RedisService(IConfiguration configuration)
        {
            var connectionString = configuration["Redis:ConnectionString"];
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                _enabled = false;
                return;
            }

            var redis = ConnectionMultiplexer.Connect(connectionString);
            _database = redis.GetDatabase();
            _enabled = true;
        }

        public bool IsEnabled => _enabled;

        public async Task SetStringAsync(string key, string value, TimeSpan? ttl = null)
        {
            if (!_enabled || _database is null) return;
            await _database.StringSetAsync(key, value, ttl);
        }

        public async Task<string?> GetStringAsync(string key)
        {
            if (!_enabled || _database is null) return null;
            var value = await _database.StringGetAsync(key);
            return value.HasValue ? value.ToString() : null;
        }

        public async Task RemoveAsync(string key)
        {
            if (!_enabled || _database is null) return;
            await _database.KeyDeleteAsync(key);
        }
    }
}

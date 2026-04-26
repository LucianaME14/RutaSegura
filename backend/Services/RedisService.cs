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
                Console.WriteLine("❌ Redis NO habilitado: falta Redis:ConnectionString");
                _enabled = false;
                return;
            }

            try
            {
                Console.WriteLine("🔄 Intentando conectar a Redis...");
                var redis = ConnectionMultiplexer.Connect(connectionString);

                _database = redis.GetDatabase();
                _enabled = true;

                Console.WriteLine("✅ Redis conectado correctamente");
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error conectando a Redis:");
                Console.WriteLine(ex.Message);
                _enabled = false;
            }
        }

        public bool IsEnabled => _enabled;

        public async Task SetStringAsync(string key, string value, TimeSpan? ttl = null)
        {
            if (!_enabled || _database is null)
            {
                Console.WriteLine($"⚠️ Redis deshabilitado. No se guardó: {key}");
                return;
            }

            Console.WriteLine($"🟢 Redis SET: {key}");

            try
            {
                await _database.StringSetAsync(key, value, ttl);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en Redis SET ({key}): {ex.Message}");
            }
        }

        public async Task<string?> GetStringAsync(string key)
        {
            if (!_enabled || _database is null)
            {
                Console.WriteLine($"⚠️ Redis deshabilitado. No se leyó: {key}");
                return null;
            }

            try
            {
                var value = await _database.StringGetAsync(key);

                if (value.HasValue)
                {
                    Console.WriteLine($"🟢 Redis HIT: {key}");
                    return value.ToString();
                }
                else
                {
                    Console.WriteLine($"🟡 Redis MISS: {key}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en Redis GET ({key}): {ex.Message}");
                return null;
            }
        }

        public async Task RemoveAsync(string key)
        {
            if (!_enabled || _database is null)
            {
                Console.WriteLine($"⚠️ Redis deshabilitado. No se borró: {key}");
                return;
            }

            Console.WriteLine($"🔴 Redis REMOVE: {key}");

            try
            {
                await _database.KeyDeleteAsync(key);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en Redis REMOVE ({key}): {ex.Message}");
            }
        }
    }
}
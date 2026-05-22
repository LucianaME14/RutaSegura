using StackExchange.Redis;

namespace RutaSegura.Services;

public class RedisService
{
    private readonly IDatabase? _database;
    private readonly IConnectionMultiplexer? _multiplexer;
    private readonly bool _configured;
    private volatile bool _operational;
    private readonly ILogger<RedisService> _logger;
    private readonly object _disableLock = new();
    private readonly string? _startupError;

    public RedisService(IConfiguration configuration, ILogger<RedisService> logger)
    {
        _logger = logger;
        var connectionString = BuildConnectionString(configuration);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            _logger.LogWarning(
                "Redis deshabilitado: falta Redis:ConnectionString (env Redis__ConnectionString). La app usará solo SQLite.");
            _configured = false;
            _operational = false;
            _startupError = "Variable Redis__ConnectionString vacía o con nombre incorrecto.";
            return;
        }

        _configured = true;

        try
        {
            var options = ConfigurationOptions.Parse(connectionString);
            var isLocal = IsLocalEndpoint(options);
            options.AbortOnConnectFail = isLocal;
            options.ConnectTimeout = isLocal ? 2500 : 15000;
            options.SyncTimeout = isLocal ? 2500 : 8000;
            options.AsyncTimeout = isLocal ? 2500 : 8000;
            options.ConnectRetry = isLocal ? 0 : 3;
            // SSL solo si la cadena trae ssl=True o rediss:// (Redis Cloud :14083 suele ser sin TLS).

            var redis = ConnectionMultiplexer.Connect(options);
            var db = redis.GetDatabase();
            var pingMs = db.Ping().TotalMilliseconds;

            _multiplexer = redis;
            _database = db;
            _operational = true;
            _logger.LogInformation(
                "Redis operativo ({Host}, ping {PingMs:F0} ms)",
                options.EndPoints.FirstOrDefault()?.ToString() ?? "redis",
                pingMs);
        }
        catch (Exception ex)
        {
            _operational = false;
            _startupError = ex.Message;
            _logger.LogWarning(
                ex,
                "Redis no disponible. Revisa Redis__ConnectionString (Internal URL de Render, formato redis://red-xxx:6379). La app seguirá con SQLite.");
        }
    }

    /// <summary>Variable configurada (aunque la conexión haya fallado).</summary>
    public bool IsConfigured => _configured;

    /// <summary>Redis configurado y respondiendo ping.</summary>
    public bool IsEnabled => _configured && _operational;

    /// <summary>Mensaje para panel admin si la conexión falló al arrancar.</summary>
    public string GetStatusMessage()
    {
        if (!_configured)
            return "Redis inactivo — falta Redis__ConnectionString en el servidor.";
        if (_operational)
            return "Redis operativo (caché y sesiones)";
        return string.IsNullOrEmpty(_startupError)
            ? "Redis inactivo — no se pudo conectar (revisa Internal URL y región)."
            : $"Redis inactivo — {_startupError}";
    }

    internal static string BuildConnectionString(IConfiguration configuration)
    {
        var raw = configuration["Redis:ConnectionString"]?.Trim();
        if (string.IsNullOrWhiteSpace(raw))
            return string.Empty;

        var extraPassword = configuration["Redis:Password"]?.Trim();

        if (raw.StartsWith("redis://", StringComparison.OrdinalIgnoreCase)
            || raw.StartsWith("rediss://", StringComparison.OrdinalIgnoreCase))
        {
            return FromRedisUri(raw, extraPassword);
        }

        if (raw.Contains('@'))
            return FromRedisUri(raw.Contains("://") ? raw : $"redis://{raw}", extraPassword);

        if (raw.Contains("password=", StringComparison.OrdinalIgnoreCase))
            return EnsureCloudOptions(raw);

        var password = extraPassword;
        if (!raw.Contains(',') && raw.Contains(':'))
        {
            var colon = raw.LastIndexOf(':');
            if (colon > 0 && int.TryParse(raw[(colon + 1)..], out var port))
            {
                var host = raw[..colon];
                var isTls = port is 6380 or 18100;
                var isRender = host.StartsWith("red-", StringComparison.OrdinalIgnoreCase)
                    || host.Contains("render.com", StringComparison.OrdinalIgnoreCase);
                var isCloud = host.Contains("redislabs.com", StringComparison.OrdinalIgnoreCase)
                    || host.Contains("redis-cloud.com", StringComparison.OrdinalIgnoreCase);

                var parts = new List<string> { $"{host}:{port}" };
                if (!string.IsNullOrEmpty(password))
                {
                    if (isCloud)
                        parts.Add("user=default");
                    parts.Add($"password={password}");
                }
                if (isTls || UseTlsFromConfig(configuration))
                    parts.Add("ssl=True");
                if (!IsLocalHost(host))
                    parts.Add("abortConnect=false");
                return string.Join(',', parts);
            }
        }

        if (raw.StartsWith("localhost", StringComparison.OrdinalIgnoreCase)
            || raw.StartsWith("127.0.0.1", StringComparison.OrdinalIgnoreCase))
        {
            return raw.Contains(',') ? raw : $"{raw},abortConnect=true";
        }

        return EnsureCloudOptions(raw);
    }

    private static string FromRedisUri(string uriString, string? extraPassword)
    {
        if (!uriString.Contains("://", StringComparison.Ordinal))
            uriString = $"redis://{uriString}";

        if (!Uri.TryCreate(uriString, UriKind.Absolute, out var uri))
            return EnsureCloudOptions(uriString);

        var host = uri.Host;
        if (string.IsNullOrEmpty(host))
            return EnsureCloudOptions(uriString);

        var port = uri.Port > 0 ? uri.Port : uri.Scheme.Equals("rediss", StringComparison.OrdinalIgnoreCase) ? 6380 : 6379;
        var parts = new List<string> { $"{host}:{port}" };

        string? password = null;
        if (!string.IsNullOrEmpty(uri.UserInfo))
        {
            var segments = uri.UserInfo.Split(':', 2);
            password = segments.Length == 2 ? Uri.UnescapeDataString(segments[1]) : null;
        }

        if (string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(extraPassword))
            password = extraPassword;

        var isCloudHost = host.Contains("redislabs.com", StringComparison.OrdinalIgnoreCase)
            || host.Contains("redis-cloud.com", StringComparison.OrdinalIgnoreCase);

        if (!string.IsNullOrEmpty(password))
        {
            if (isCloudHost && (string.IsNullOrEmpty(uri.UserInfo) || uri.UserInfo.StartsWith("default:", StringComparison.OrdinalIgnoreCase)))
                parts.Add("user=default");
            parts.Add($"password={password}");
        }

        if (uri.Scheme.Equals("rediss", StringComparison.OrdinalIgnoreCase))
            parts.Add("ssl=True");

        if (!IsLocalHost(host))
            parts.Add("abortConnect=false");

        return string.Join(',', parts);
    }

    private static string EnsureCloudOptions(string value)
    {
        if (IsLocalHost(value) || value.Contains("abortConnect", StringComparison.OrdinalIgnoreCase))
            return value;
        return $"{value},abortConnect=false";
    }

    private static bool IsLocalHost(string host) =>
        host.StartsWith("localhost", StringComparison.OrdinalIgnoreCase)
        || host.StartsWith("127.0.0.1", StringComparison.OrdinalIgnoreCase);

    private static bool IsLocalEndpoint(ConfigurationOptions options)
    {
        foreach (var ep in options.EndPoints)
        {
            var s = ep.ToString() ?? "";
            if (IsLocalHost(s.Split(':')[0]))
                return true;
        }
        return false;
    }

    private static bool UseTlsFromConfig(IConfiguration configuration) =>
        string.Equals(configuration["Redis:Ssl"], "true", StringComparison.OrdinalIgnoreCase)
        || string.Equals(configuration["Redis:UseSsl"], "true", StringComparison.OrdinalIgnoreCase);

    private void DisableAfterFailure(Exception ex, string operation, string key)
    {
        lock (_disableLock)
        {
            if (!_operational) return;
            _operational = false;
            _logger.LogWarning(
                ex,
                "Redis dejó de responder ({Operation} {Key}). Caché desactivada; usando SQLite.",
                operation,
                key);
        }

        try
        {
            _multiplexer?.Dispose();
        }
        catch
        {
            /* ignore */
        }
    }

    public async Task SetStringAsync(string key, string value, TimeSpan? ttl = null)
    {
        if (!IsEnabled || _database is null) return;

        try
        {
            await _database.StringSetAsync(key, value, ttl);
        }
        catch (Exception ex)
        {
            DisableAfterFailure(ex, "SET", key);
        }
    }

    public async Task<string?> GetStringAsync(string key)
    {
        if (!IsEnabled || _database is null) return null;

        try
        {
            var value = await _database.StringGetAsync(key);
            return value.HasValue ? value.ToString() : null;
        }
        catch (Exception ex)
        {
            DisableAfterFailure(ex, "GET", key);
        }

        return null;
    }

    public async Task RemoveAsync(string key)
    {
        if (!IsEnabled || _database is null) return;

        try
        {
            await _database.KeyDeleteAsync(key);
        }
        catch (Exception ex)
        {
            DisableAfterFailure(ex, "REMOVE", key);
        }
    }
}

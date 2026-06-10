namespace RutaSegura.AI.Services;

public record OllamaStatusDto(
    bool Disponible,
    string BaseUrl,
    string Modelo,
    string Mensaje,
    IReadOnlyList<string>? ModelosInstalados);

public interface IOllamaService
{
    string BaseUrl { get; }
    string CurrentModel { get; }
    Task<bool> IsAvailableAsync(CancellationToken ct = default);
    Task<OllamaStatusDto> GetStatusAsync(CancellationToken ct = default);
}

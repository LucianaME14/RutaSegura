namespace RutaSegura.AI.Services;

public record LlmStatusDto(
    bool Disponible,
    string Proveedor,
    string Modelo,
    string Mensaje,
    IReadOnlyList<string>? ModelosInstalados = null);

public interface ILlmStatusService
{
    string Proveedor { get; }
    string CurrentModel { get; }
    bool UsaGroq { get; }
    Task<bool> IsLlmAvailableAsync(CancellationToken ct = default);
    Task<LlmStatusDto> GetStatusAsync(CancellationToken ct = default);
}

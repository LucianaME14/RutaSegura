using System.ComponentModel;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using RutaSegura.Data;

namespace RutaSegura.AI.Plugins;

/// <summary>Plugin Semantic Kernel: métricas administrativas del sistema.</summary>
public class AdminPlugin
{
    private readonly ApplicationDbContext _db;

    public AdminPlugin(ApplicationDbContext db) => _db = db;

    [KernelFunction("resumen_sistema")]
    [Description("Resumen para administradores: usuarios, reportes, alertas y zonas críticas.")]
    public async Task<string> ResumenSistemaAsync()
    {
        var hoy = DateTime.UtcNow.Date;
        var usuarios = await _db.Usuarios.CountAsync();
        var reportesHoy = await _db.Reportes.CountAsync(r => r.FechaReporte >= hoy);
        var reportesTotal = await _db.Reportes.CountAsync();
        var pendientes = await _db.Reportes.CountAsync(r => r.Estado == "Pendiente");
        var alertas = await _db.AlertasSistema.CountAsync();

        var zonasPeligrosas = await _db.Reportes
            .AsNoTracking()
            .Where(r => r.FechaReporte >= DateTime.UtcNow.AddDays(-60) && r.Estado != "Rechazado")
            .GroupBy(r => r.Ubicacion)
            .Select(g => new { zona = g.Key, cantidad = g.Count() })
            .OrderByDescending(x => x.cantidad)
            .Take(5)
            .ToListAsync();

        return JsonSerializer.Serialize(new
        {
            usuarios_registrados = usuarios,
            reportes_hoy = reportesHoy,
            reportes_total = reportesTotal,
            reportes_pendientes = pendientes,
            alertas_sistema = alertas,
            zonas_mas_incidentes = zonasPeligrosas,
        });
    }
}

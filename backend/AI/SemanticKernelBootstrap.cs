using Microsoft.SemanticKernel;
using Microsoft.Extensions.Options;
using RutaSegura.AI.Options;
using RutaSegura.AI.Plugins;
using RutaSegura.AI.Services;

#pragma warning disable SKEXP0070

namespace RutaSegura.AI;

public static class SemanticKernelBootstrap
{
    public static IServiceCollection AddRutaSeguraAi(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<OllamaOptions>(config.GetSection(OllamaOptions.SectionName));

        var ollamaSection = config.GetSection(OllamaOptions.SectionName);
        var baseUrl = ollamaSection["BaseUrl"] ?? "http://localhost:11434";
        var model = ollamaSection["Model"] ?? "llama3";

        services.AddHttpClient("ollama", c =>
        {
            c.Timeout = TimeSpan.FromSeconds(
                int.TryParse(ollamaSection["TimeoutSeconds"], out var t) ? t : 120);
        });

        services.AddSingleton<IOllamaService, OllamaService>();
        services.AddSingleton<IChatCacheService, ChatCacheService>();
        services.AddSingleton<Memory.ChatSessionMemory>();

        services.AddScoped<ReportesPlugin>();
        services.AddScoped<RutasPlugin>();
        services.AddScoped<AlertasPlugin>();
        services.AddScoped<MlPlugin>();
        services.AddScoped<ClimaPlugin>();
        services.AddScoped<AdminPlugin>();
        services.AddScoped<ExplicacionesPlugin>();
        services.AddScoped<SafeBotContextBuilder>();
        services.AddScoped<ISafeBotAgentService, SafeBotAgentService>();

        services.AddScoped<Kernel>(sp =>
        {
            var opts = sp.GetRequiredService<IOptions<OllamaOptions>>().Value;
            var builder = Kernel.CreateBuilder();
            builder.AddOllamaChatCompletion(
                modelId: opts.Model,
                endpoint: new Uri(opts.BaseUrl.TrimEnd('/')));
            var kernel = builder.Build();

            kernel.ImportPluginFromObject(sp.GetRequiredService<ReportesPlugin>(), "Reportes");
            kernel.ImportPluginFromObject(sp.GetRequiredService<RutasPlugin>(), "Rutas");
            kernel.ImportPluginFromObject(sp.GetRequiredService<AlertasPlugin>(), "Alertas");
            kernel.ImportPluginFromObject(sp.GetRequiredService<MlPlugin>(), "ML");
            kernel.ImportPluginFromObject(sp.GetRequiredService<ClimaPlugin>(), "Clima");
            kernel.ImportPluginFromObject(sp.GetRequiredService<AdminPlugin>(), "Admin");
            kernel.ImportPluginFromObject(sp.GetRequiredService<ExplicacionesPlugin>(), "Explicaciones");

            return kernel;
        });

        return services;
    }
}

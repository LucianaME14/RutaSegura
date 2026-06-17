using Microsoft.SemanticKernel;
using Microsoft.Extensions.Options;
using RutaSegura.AI.Options;
using RutaSegura.AI.Plugins;
using RutaSegura.AI.Services;

#pragma warning disable SKEXP0070
#pragma warning disable SKEXP0010

namespace RutaSegura.AI;

public static class SemanticKernelBootstrap
{
    public static IServiceCollection AddRutaSeguraAi(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<OllamaOptions>(config.GetSection(OllamaOptions.SectionName));
        services.Configure<GroqOptions>(config.GetSection(GroqOptions.SectionName));

        var ollamaSection = config.GetSection(OllamaOptions.SectionName);
        services.AddHttpClient("ollama", c =>
        {
            c.Timeout = TimeSpan.FromSeconds(
                int.TryParse(ollamaSection["TimeoutSeconds"], out var t) ? t : 120);
        });

        services.AddSingleton<IOllamaService, OllamaService>();
        services.AddSingleton<ILlmStatusService, LlmStatusService>();
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
            var groq = sp.GetRequiredService<IOptions<GroqOptions>>().Value;
            var ollama = sp.GetRequiredService<IOptions<OllamaOptions>>().Value;
            var builder = Kernel.CreateBuilder();

            // Prioridad: Groq (Render / API key) → Ollama (local)
            if (groq.Enabled && !string.IsNullOrWhiteSpace(groq.ApiKey))
            {
                builder.AddOpenAIChatCompletion(
                    modelId: groq.Model,
                    apiKey: groq.ApiKey,
                    endpoint: new Uri(groq.BaseUrl.TrimEnd('/')));
            }
            else if (ollama.Enabled)
            {
                builder.AddOllamaChatCompletion(
                    modelId: ollama.Model,
                    endpoint: new Uri(ollama.BaseUrl.TrimEnd('/')));
            }

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

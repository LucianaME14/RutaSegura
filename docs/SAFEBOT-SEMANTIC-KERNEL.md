# SafeBot — Microsoft Semantic Kernel + Ollama + ML.NET

Documentación para exposición universitaria del asistente inteligente **SafeBot** en Ruta Segura.

## Arquitectura

```
Usuario (React SafeBot.tsx)
        │ POST /api/chat  (JWT)
        ▼
ChatController
        ▼
SafeBotAgentService  ←── ChatSessionMemory (historial)
        │
        ├─► ChatCacheService (Redis: respuestas frecuentes)
        ├─► SafeBotContextBuilder (intención → plugins)
        │
        ├─► Plugins Semantic Kernel
        │     ├─ ReportesPlugin   (SQLite + Redis)
        │     ├─ RutasPlugin      (ML.NET + historial)
        │     ├─ AlertasPlugin    (alertas + riesgo)
        │     ├─ MlPlugin         (clasificación zona)
        │     ├─ ClimaPlugin      (WeatherAPI)
        │     └─ AdminPlugin      (métricas admin)
        │
        └─► Microsoft Semantic Kernel
              └─► Ollama (llama3) → respuesta natural en español
```

## Estructura de carpetas

```
backend/
├── AI/
│   ├── Options/
│   │   └── OllamaOptions.cs
│   ├── Plugins/
│   │   ├── ReportesPlugin.cs      # PluginReportes
│   │   ├── RutasPlugin.cs         # PluginRutas
│   │   ├── AlertasPlugin.cs       # PluginAlertas
│   │   ├── MlPlugin.cs            # PluginML
│   │   ├── ClimaPlugin.cs         # PluginClima
│   │   └── AdminPlugin.cs         # métricas administrador
│   ├── Services/
│   │   ├── IOllamaService.cs
│   │   ├── OllamaService.cs
│   │   ├── ChatCacheService.cs
│   │   ├── SafeBotContextBuilder.cs
│   │   └── SafeBotAgentService.cs
│   ├── Prompts/
│   │   └── SafeBotPrompts.cs
│   ├── Memory/
│   │   └── ChatSessionMemory.cs
│   └── SemanticKernelBootstrap.cs
└── Controllers/
    └── ChatController.cs

src/app/components/
└── SafeBot.tsx                    # UI chat conectada al API
```

## Configuración Ollama

En `.env` (raíz del proyecto):

```env
Ollama__BaseUrl=http://localhost:11434
Ollama__Model=llama3
Ollama__Enabled=true
Ollama__TimeoutSeconds=120
```

Modelos soportados (cambiar `Ollama__Model`):

| Modelo        | Comando              |
|---------------|----------------------|
| llama3        | `ollama pull llama3` |
| mistral       | `ollama pull mistral` |
| deepseek-r1   | `ollama pull deepseek-r1` |
| phi3          | `ollama pull phi3` |

Arranque local:

```bash
ollama serve
ollama pull llama3
```

## Registro en Program.cs

```csharp
builder.Services.AddRutaSeguraAi(builder.Configuration);
```

Esto registra: `IOllamaService`, plugins, `Kernel` (Semantic Kernel), `ISafeBotAgentService` y caché Redis.

## API

### POST `/api/chat` (requiere JWT)

**Request:**

```json
{
  "message": "¿Es segura esta zona?",
  "zona": "Miraflores",
  "origen": "Mi casa",
  "destino": "Universidad"
}
```

**Response:**

```json
{
  "answer": "La zona presenta riesgo moderado según los reportes...",
  "llmActivo": true,
  "modelo": "llama3",
  "desdeCache": false,
  "fuentes": ["PluginML", "PluginAlertas"]
}
```

### GET `/api/chat/status`

Estado de Ollama (diagnóstico).

## Flujo ejemplo: «¿Esta zona es segura?»

1. Usuario envía mensaje desde SafeBot.
2. `SafeBotContextBuilder` detecta intención de seguridad/zona.
3. `MlPlugin.ClasificarZona()` ejecuta ML.NET (Segura / Moderada / Peligrosa).
4. `AlertasPlugin.ObtenerNivelRiesgo()` consulta SQLite.
5. Contexto JSON se pasa a Semantic Kernel.
6. Ollama (`llama3`) genera respuesta natural en español.
7. Respuesta se guarda en Redis (caché 10 min) y memoria de sesión.

## Plugins y funciones

| Plugin | Funciones SK |
|--------|----------------|
| **Reportes** | `obtener_reportes_recientes`, `obtener_cantidad_reportes`, `obtener_reportes_por_zona` |
| **Rutas** | `obtener_ruta_mas_segura`, `obtener_ruta_mas_rapida`, `obtener_ruta_equilibrada` |
| **Alertas** | `obtener_alertas_activas`, `obtener_nivel_riesgo` |
| **ML** | `clasificar_zona`, `recomendar_ruta` |
| **Clima** | `obtener_clima_actual` |
| **Admin** | `resumen_sistema` |

## Usuario vs Administrador

| Pregunta tipo | Usuario | Admin |
|---------------|---------|-------|
| ¿Es segura esta zona? | ✅ ML + reportes | ✅ |
| Ruta más segura | ✅ ML.NET | ✅ |
| Reportes cercanos | ✅ | ✅ |
| ¿Cómo funciona SOS? | ✅ | ✅ |
| Clima actual | ✅ | ✅ |
| ¿Cuántos reportes hoy? | ✅ cantidad | ✅ + resumen |
| Usuarios registrados | — | ✅ AdminPlugin |
| Zonas peligrosas | — | ✅ AdminPlugin |

El prompt de sistema cambia según rol (`SafeBotPrompts.System(esAdmin)`).

## Redis

| Clave | Uso |
|-------|-----|
| `safebot:chat:v1:{userId}:{hash}` | Caché respuestas chat (10 min) |
| `safebot:reportes:*` | Caché reportes |
| `safebot:ruta:*` | Caché rutas ML |
| `safebot:alertas:*` | Caché alertas |

Sin Redis, SafeBot funciona consultando SQLite directamente.

## Producción (Render)

Ollama es **local**. En Render sin Ollama, SafeBot opera en **modo datos**: plugins + respuesta estructurada sin LLM. Para demo con IA completa, ejecutar backend en máquina local con `ollama serve`.

## Paquetes NuGet

- `Microsoft.SemanticKernel` 1.49.0
- `Microsoft.SemanticKernel.Connectors.Ollama` 1.49.0-alpha

## Guion de demo (5 min)

1. Mostrar `ollama list` y `GET /api/chat/status`.
2. Login usuario → abrir SafeBot → «¿Es segura Miraflores?» → explicar flujo ML.NET + Ollama.
3. «¿Cuál es la ruta más segura a la universidad?» → PluginRutas + ML.
4. Login admin → «¿Cuántos reportes hay hoy?» → AdminPlugin + SQLite.
5. Mencionar Redis en respuestas repetidas (`desdeCache: true`).

## Ejemplos de prompts

- ¿Es segura esta zona?
- ¿Cuál es la ruta más segura?
- ¿Qué reportes hay cerca?
- ¿Cómo funciona SOS?
- ¿Qué zonas tienen más incidentes?
- ¿Cuántos reportes existen hoy?
- ¿Cuál es el clima actual?

namespace RutaSegura.ML;

/// <summary>
/// Rutas estándar para ML.NET Model Builder y despliegue (Docker/Render).
/// Copia los .zip generados en Visual Studio a <c>backend/Models/</c>.
/// </summary>
public static class MlModelPaths
{
    public const string ZoneClassifierFile = "zone-safety-classifier.zip";
    public const string RouteRecommenderFile = "route-recommender.zip";
    public const string IncidentClassifierFile = "incident-classifier.zip";

    public const string ZoneCsvFile = "zona_clasificacion.csv";
    public const string RoutesCsvFile = "rutas_recomendacion.csv";

    public static string ResolveModelsDir(string contentRoot)
        => Path.Combine(contentRoot, "Models");

    public static string ResolveDatasetsDir(string contentRoot)
        => Path.Combine(contentRoot, "Datasets");

    public static string ResolveLegacyArtifactsDir(string contentRoot)
        => Path.Combine(contentRoot, "ML", "Artifacts");

    public static string ZoneModelPath(string contentRoot)
        => Path.Combine(ResolveModelsDir(contentRoot), ZoneClassifierFile);

    public static string RecommenderModelPath(string contentRoot)
        => Path.Combine(ResolveModelsDir(contentRoot), RouteRecommenderFile);

    public static string IncidentModelPath(string contentRoot)
        => Path.Combine(ResolveModelsDir(contentRoot), IncidentClassifierFile);

    public static string ZoneCsvPath(string contentRoot)
        => Path.Combine(ResolveDatasetsDir(contentRoot), ZoneCsvFile);

    public static string RoutesCsvPath(string contentRoot)
        => Path.Combine(ResolveDatasetsDir(contentRoot), RoutesCsvFile);
}

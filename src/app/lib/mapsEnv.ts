/** Mismo `id` en toda la app: evita cargar el script de Google Maps varias veces. */
export const GOOGLE_MAPS_SCRIPT_ID = "rutasegura-maps";

/** Clave de Maps inyectada por Vite (requiere .env en la raíz y reiniciar `npm run dev`). */
export function getGoogleMapsApiKey(): string | undefined {
  const raw = import.meta.env.VITE_GOOGLE_MAPS_API_KEY;
  if (raw == null || raw === "") return undefined;
  const t = String(raw).trim();
  return t.length > 0 ? t : undefined;
}

export function getGoogleMapsLoaderConfig(apiKey: string) {
  return {
    id: GOOGLE_MAPS_SCRIPT_ID,
    googleMapsApiKey: apiKey,
    version: "weekly" as const,
    language: "es",
    // Necesario para autocompletado / SearchBox (buscador de direcciones)
    libraries: ["places" as const],
  };
}

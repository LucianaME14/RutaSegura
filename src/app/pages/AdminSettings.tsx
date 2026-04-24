import { useCallback, useEffect, useState } from "react";
import { Settings, Database, MapPin } from "lucide-react";
import { apiUrl } from "../lib/api";

type ConfigGet = {
  id: number;
  pesoZonasOscurasPct: number;
  caducidadReporteMenorHoras: number;
  autoAprobarConfianzaMinPct: number;
  pushNotificacionUrl?: string | null;
  hasGoogleKey: boolean;
};

export default function AdminSettings() {
  const [apiKey, setApiKey] = useState("");
  const [darkZoneWeight, setDarkZoneWeight] = useState(40);
  const [reportTTL, setReportTTL] = useState(24);
  const [autoApproveThreshold, setAutoApproveThreshold] = useState(85);
  const [pushUrl, setPushUrl] = useState("https://push.rutasegura.net");
  const [loading, setLoading] = useState(true);
  const [msg, setMsg] = useState<string | null>(null);
  const [dbMsg, setDbMsg] = useState<string | null>(null);
  const [hasServerKey, setHasServerKey] = useState(false);

  const load = useCallback(() => {
    setLoading(true);
    fetch(apiUrl("/api/Admin/configuracion"))
      .then((r) => {
        if (!r.ok) throw new Error();
        return r.json() as Promise<ConfigGet>;
      })
      .then((c) => {
        setDarkZoneWeight(c.pesoZonasOscurasPct);
        setReportTTL(c.caducidadReporteMenorHoras);
        setAutoApproveThreshold(c.autoAprobarConfianzaMinPct);
        if (c.pushNotificacionUrl) setPushUrl(c.pushNotificacionUrl);
        setHasServerKey(!!c.hasGoogleKey);
        setApiKey("");
      })
      .catch(() => setMsg("No se pudo cargar la configuración."))
      .finally(() => setLoading(false));
  }, []);

  useEffect(() => {
    load();
  }, [load]);

  const guardar = async () => {
    setMsg(null);
    const body: Record<string, unknown> = {
      pesoZonasOscurasPct: darkZoneWeight,
      caducidadReporteMenorHoras: reportTTL,
      autoAprobarConfianzaMinPct: autoApproveThreshold,
      pushNotificacionUrl: pushUrl,
    };
    const t = apiKey.trim();
    if (t) body.googleMapsKeyAlmacenada = t;

    const r = await fetch(apiUrl("/api/Admin/configuracion"), {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(body),
    });
    if (r.ok) {
      setMsg("Cambios guardados en la base de datos.");
      load();
    } else {
      setMsg("Error al guardar.");
    }
  };

  const probarDb = async () => {
    setDbMsg(null);
    const r = await fetch(apiUrl("/api/Admin/db-health"));
    const j = await r.json();
    setDbMsg(j?.message || (j?.ok ? "OK" : "Error"));
  };

  if (loading) {
    return (
      <p className="text-slate-500 py-10">Cargando configuración…</p>
    );
  }

  return (
    <div className="space-y-6 animate-in fade-in duration-500 pb-10">
      {msg ? (
        <div className="rounded-2xl border border-indigo-200 bg-indigo-50 px-4 py-3 text-sm text-indigo-900">
          {msg}
        </div>
      ) : null}
      <div>
        <h1 className="text-3xl font-black text-slate-900">
          Configuración del Sistema
        </h1>
        <p className="mt-2 text-slate-500">
          Valores persistidos en la tabla <code>ConfiguracionSistema</code>{" "}
          (fila Id=1).
        </p>
      </div>

      <div className="grid gap-6 xl:grid-cols-2">
        <div className="rounded-[2rem] border border-slate-200 bg-white p-6 shadow-sm">
          <div className="flex items-center gap-3 mb-6">
            <div className="rounded-3xl bg-indigo-50 p-3 text-indigo-600">
              <Settings className="w-5 h-5" />
            </div>
            <div>
              <h2 className="text-xl font-bold text-slate-900">
                Parámetros del Algoritmo (ML)
              </h2>
              <p className="text-sm text-slate-500">
                Se usan como referencia en el panel; el motor de negocio puede
                leerlos de la API.
              </p>
            </div>
          </div>

          <div className="space-y-5">
            <div>
              <label className="text-sm font-bold text-slate-700">
                Peso de zonas oscuras en Score (%)
              </label>
              <input
                id="dark-zone-weight"
                type="number"
                value={darkZoneWeight}
                onChange={(e) => setDarkZoneWeight(Number(e.target.value))}
                placeholder="40"
                title="Peso de zonas oscuras en score"
                className="mt-3 w-full rounded-3xl border border-slate-200 bg-slate-100 px-4 py-3 text-sm outline-none focus:border-indigo-500 focus:ring-2 focus:ring-indigo-100"
              />
            </div>
            <div>
              <label
                htmlFor="report-ttl"
                className="text-sm font-bold text-slate-700"
              >
                Tiempo caducidad de reportes menores (horas)
              </label>
              <input
                id="report-ttl"
                type="number"
                value={reportTTL}
                onChange={(e) => setReportTTL(Number(e.target.value))}
                placeholder="24"
                title="Tiempo de caducidad de reportes menores"
                className="mt-3 w-full rounded-3xl border border-slate-200 bg-slate-100 px-4 py-3 text-sm outline-none focus:border-indigo-500 focus:ring-2 focus:ring-indigo-100"
              />
            </div>
            <div>
              <label className="text-sm font-bold text-slate-700">
                Nivel confianza mínimo de IA para auto-aprobar
              </label>
              <select
                value={autoApproveThreshold}
                onChange={(e) =>
                  setAutoApproveThreshold(Number(e.target.value))
                }
                className="mt-3 w-full rounded-3xl border border-slate-200 bg-slate-100 px-4 py-3 text-sm outline-none focus:border-indigo-500 focus:ring-2 focus:ring-indigo-100"
              >
                {[75, 80, 85, 90, 95].map((value) => (
                  <option key={value} value={value}>
                    {value}%
                  </option>
                ))}
              </select>
            </div>
          </div>
        </div>

        <div className="rounded-[2rem] border border-slate-200 bg-white p-6 shadow-sm">
          <div className="flex items-center gap-3 mb-6">
            <div className="rounded-3xl bg-slate-100 p-3 text-slate-700">
              <MapPin className="w-5 h-5" />
            </div>
            <div>
              <h2 className="text-xl font-bold text-slate-900">
                Integraciones de API
              </h2>
              <p className="text-sm text-slate-500">
                El mapa en el cliente sigue usando <code>VITE_GOOGLE_MAPS_API_KEY</code>{" "}
                en <code>.env</code>. Aquí puedes almacenar otra clave en servidor
                si luego usas un proxy.
              </p>
            </div>
          </div>

          <div className="space-y-5">
            <p className="text-xs text-slate-500">
              {hasServerKey
                ? "Ya hay una clave guardada en el servidor. Escribe de nuevo para reemplazar; déjala vacía y guarda para no reemplazar (usa el botón y envía clave en blanco para borrar, si lo implementas)."
                : "No hay clave en el servidor aún."}
            </p>
            <div>
              <label
                htmlFor="google-maps-api-key"
                className="text-sm font-bold text-slate-700"
              >
                Google Maps API Key (almacenamiento en BD, opcional)
              </label>
              <input
                id="google-maps-api-key"
                type="password"
                value={apiKey}
                onChange={(e) => setApiKey(e.target.value)}
                placeholder="Dejar en blanco para no cambiar"
                title="Clave de API de Google Maps (servidor)"
                className="mt-3 w-full rounded-3xl border border-slate-200 bg-slate-100 px-4 py-3 text-sm outline-none focus:border-indigo-500 focus:ring-2 focus:ring-indigo-100"
              />
            </div>
            <div>
              <label
                htmlFor="push-server"
                className="text-sm font-bold text-slate-700"
              >
                Servidor de Notificaciones (Push)
              </label>
              <input
                id="push-server"
                type="text"
                value={pushUrl}
                onChange={(e) => setPushUrl(e.target.value)}
                placeholder="https://push.rutasegura.net"
                title="URL del servidor de notificaciones push"
                className="mt-3 w-full rounded-3xl border border-slate-200 bg-slate-100 px-4 py-3 text-sm text-slate-800 outline-none focus:border-indigo-500 focus:ring-2 focus:ring-indigo-100"
              />
            </div>
            {dbMsg ? (
              <p className="text-sm text-slate-600">{dbMsg}</p>
            ) : null}
            <button
              type="button"
              onClick={probarDb}
              className="inline-flex items-center gap-3 rounded-3xl bg-slate-900 px-5 py-3 text-sm font-bold text-white transition hover:bg-slate-800"
            >
              <Database className="w-5 h-5" /> Probar Conexión a Base de Datos
            </button>
          </div>
        </div>
      </div>

      <button
        type="button"
        onClick={guardar}
        className="rounded-3xl bg-indigo-600 px-6 py-4 text-sm font-black text-white shadow-lg transition hover:bg-indigo-500"
      >
        Guardar Cambios Globales
      </button>
    </div>
  );
}

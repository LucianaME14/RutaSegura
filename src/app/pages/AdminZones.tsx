import { useEffect, useMemo, useState } from "react";
import {
  MapPin,
  Navigation2,
  CheckCircle2,
  ShieldAlert,
  Search,
} from "lucide-react";
import { GoogleMapView, LIMA_CENTRO, type MapaMarcadorDinamico } from "../components/GoogleMap";
import { apiUrl } from "../lib/api";
import type { LatLngLiteral } from "../types/maps";

type PuntoApi = {
  id: number;
  tipoIncidente: string;
  lat: number;
  lng: number;
  estado: string;
};

function colorPorEstado(estado: string) {
  if (estado === "Aprobado") return "#10b981";
  if (estado === "Rechazado") return "#f43f5e";
  return "#f59e0b";
}

export default function AdminZones() {
  const [puntos, setPuntos] = useState<PuntoApi[]>([]);
  const [loading, setLoading] = useState(true);
  const [hint, setHint] = useState<string | null>(null);

  useEffect(() => {
    setLoading(true);
    fetch(apiUrl("/api/Admin/puntos-mapa?maxDias=60"))
      .then((r) => r.json())
      .then((d: PuntoApi[]) => {
        setPuntos(Array.isArray(d) ? d : []);
        if (!d?.length) {
          setHint(
            "No hay reportes con latitud/longitud en la base. Al reportar, asegúrate de que se guarden coordenadas; mientras, verás el mapa demo.",
          );
        } else {
          setHint(null);
        }
      })
      .catch(() => {
        setPuntos([]);
        setHint("No se pudieron cargar los puntos.");
      })
      .finally(() => setLoading(false));
  }, []);

  const center: LatLngLiteral | undefined = useMemo(() => {
    if (puntos.length === 0) return undefined;
    return { lat: puntos[0].lat, lng: puntos[0].lng };
  }, [puntos]);

  const marcadoresDinamicos: MapaMarcadorDinamico[] | null = useMemo(() => {
    if (puntos.length === 0) return null;
    return puntos.map((p, i) => ({
      id: `r-${p.id}`,
      lat: p.lat,
      lng: p.lng,
      title: p.tipoIncidente,
      label: String(i + 1),
      color: colorPorEstado(p.estado),
      descripcion: `${p.estado} — reporte #${p.id}`,
    }));
  }, [puntos]);

  return (
    <div className="space-y-6 animate-in fade-in duration-500 pb-10">
      <div className="flex flex-col gap-4 md:flex-row md:items-end md:justify-between">
        <div>
          <h1 className="text-3xl font-black text-slate-900">Mapa de Calor</h1>
          <p className="mt-2 text-slate-500">
            Pin de cada <strong>reporte</strong> con coordenadas en la base
            (últimos 60 días). Colores: pendiente (ámbar), aprobado (verde),
            rechazado (rojo).
          </p>
          {hint ? (
            <p className="mt-2 text-sm text-amber-800 bg-amber-50 border border-amber-100 rounded-xl px-3 py-2">
              {hint}
            </p>
          ) : null}
        </div>
        <div className="relative w-full md:w-[380px]">
          <Search className="absolute left-4 top-1/2 -translate-y-1/2 w-5 h-5 text-slate-400" />
          <input
            type="text"
            readOnly
            title="Búsqueda: usar mapa o capa de Google en otras pantallas"
            placeholder="Filtro por mapa (datos desde API)…"
            className="w-full rounded-3xl border border-slate-200 bg-slate-100 px-12 py-3 text-sm text-slate-500 outline-none"
          />
        </div>
      </div>

      {loading ? (
        <p className="text-slate-500">Cargando puntos…</p>
      ) : null}

      <div className="relative overflow-hidden rounded-[2rem] border border-slate-200 bg-slate-100 shadow-sm">
        <div className="absolute inset-0 bg-white/40 backdrop-blur-sm" />
        <div className="relative h-[560px]">
          <GoogleMapView
            center={center ?? LIMA_CENTRO}
            zoom={puntos.length > 0 ? 13 : 15}
            marcadoresDinamicos={marcadoresDinamicos}
          />

          <div className="absolute top-6 right-6 z-10 flex flex-col gap-3">
            <button
              type="button"
              className="inline-flex items-center gap-2 rounded-3xl bg-white px-4 py-3 text-sm font-bold text-indigo-700 shadow-lg shadow-slate-200 transition hover:bg-slate-50"
            >
              <Navigation2 className="w-5 h-5" /> Compartir Trayecto
            </button>
          </div>

          <div className="absolute bottom-6 left-6 z-10 flex flex-col gap-3">
            <button
              type="button"
              className="inline-flex items-center gap-2 rounded-3xl bg-emerald-600 px-5 py-3 text-sm font-black text-white shadow-lg shadow-emerald-200 transition hover:bg-emerald-700"
            >
              <CheckCircle2 className="w-5 h-5" /> Llegué Bien
            </button>
          </div>

          <div className="absolute bottom-6 right-6 z-10 flex flex-col items-end gap-3">
            <button
              type="button"
              className="inline-flex items-center justify-center rounded-3xl bg-white p-3.5 text-slate-700 shadow-lg shadow-slate-200 transition hover:bg-slate-50"
            >
              <MapPin className="w-6 h-6" />
            </button>
            <button
              type="button"
              className="inline-flex items-center gap-2 rounded-3xl bg-red-600 px-5 py-3 text-sm font-black text-white shadow-lg shadow-red-200 transition hover:bg-red-700"
            >
              <ShieldAlert className="w-5 h-5" /> SOS
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}

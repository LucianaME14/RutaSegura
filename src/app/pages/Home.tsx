import { useCallback, useEffect, useState } from "react";
import {
  Navigation,
  MapPin,
  AlertTriangle,
  Clock,
  ShieldAlert,
  Loader2,
} from "lucide-react";
import { Link } from "react-router";
import { formatDistanceToNow } from "date-fns";
import { es } from "date-fns/locale";
import { useAuth } from "../contexts/AuthContext";
import { apiUrl } from "../lib/api";

type AlertaRecienteApi = {
  id: number;
  tipoIncidente: string;
  ubicacion: string;
  descripcion: string | null;
  fechaReporte: string;
};

function truncar(s: string, max: number) {
  const t = s.trim();
  if (t.length <= max) return t;
  return `${t.slice(0, max - 1).trimEnd()}…`;
}

function classAlerta(tipo: string) {
  if (!tipo || typeof tipo !== "string") {
    return {
      box: "border border-slate-200 bg-slate-50",
      iconBg: "bg-slate-600",
      title: "text-slate-900",
      body: "text-slate-600",
      time: "text-slate-500",
    };
  }
  const t = stripAccents(tipo?.toLowerCase() || "otro");
  if (t.includes("robo")) {
    return {
      box: "border border-red-100 bg-red-50",
      iconBg: "bg-red-500",
      title: "text-red-900",
      body: "text-red-700",
      time: "text-red-600",
    };
  }
  if (t.includes("acoso")) {
    return {
      box: "border border-violet-100 bg-violet-50",
      iconBg: "bg-violet-600",
      title: "text-violet-950",
      body: "text-violet-800",
      time: "text-violet-600",
    };
  }
  if (t.includes("accidente")) {
    return {
      box: "border border-orange-100 bg-orange-50",
      iconBg: "bg-orange-500",
      title: "text-orange-950",
      body: "text-orange-800",
      time: "text-orange-600",
    };
  }
  if (
    t.includes("iluminac") ||
    t.includes("hueco") ||
    t.includes("via") ||
    t.includes("vía")
  ) {
    return {
      box: "border border-amber-100 bg-amber-50",
      iconBg: "bg-amber-500",
      title: "text-amber-900",
      body: "text-amber-700",
      time: "text-amber-600",
    };
  }
  if (t.includes("otro") || t.includes("peligro")) {
    return {
      box: "border border-slate-200 bg-slate-50",
      iconBg: "bg-slate-600",
      title: "text-slate-900",
      body: "text-slate-600",
      time: "text-slate-500",
    };
  }
  return {
    box: "border border-indigo-100 bg-indigo-50/80",
    iconBg: "bg-indigo-600",
    title: "text-indigo-950",
    body: "text-indigo-800/90",
    time: "text-indigo-600",
  };
}

function stripAccents(s: string) {
  if (!s || typeof s !== "string") return "";
  return s.normalize("NFD").replace(/[\u0300-\u036f]/g, "");
}

function usarIconoEscudo(tipo: string) {
  if (!tipo || typeof tipo !== "string") return false;
  const t = stripAccents(tipo?.toLowerCase() || "otro");
  return (
    t.includes("robo") ||
    t.includes("acoso") ||
    t.includes("accidente") ||
    t.includes("otro")
  );
}

function textoRelativo(fechaIso: string) {
  try {
    return formatDistanceToNow(new Date(fechaIso), {
      addSuffix: true,
      locale: es,
    });
  } catch {
    return "Reciente";
  }
}

export default function Home() {
  const { user } = useAuth();
  const [alertas, setAlertas] = useState<AlertaRecienteApi[]>([]);
  const [cargandoAlertas, setCargandoAlertas] = useState(true);
  const [errorAlertas, setErrorAlertas] = useState<string | null>(null);
  const saludo = user?.nombre?.trim().split(/\s+/)[0] ?? "explorador";

  const fetchAlertas = useCallback(async () => {
    setCargandoAlertas(true);
    setErrorAlertas(null);
    try {
      const r = await fetch(
        apiUrl("/api/Reportes/recientes?take=8&maxDays=30"),
      );
      if (!r.ok) {
        setErrorAlertas("No se pudieron cargar las alertas.");
        setAlertas([]);
        return;
      }
      const data: AlertaRecienteApi[] = await r.json();
      setAlertas(
        Array.isArray(data)
          ? data.filter((a) => a && a.id && a.ubicacion && a.fechaReporte)
          : [],
      );
    } catch {
      setErrorAlertas(
        "Revisa la conexión; no se pudo cargar el listado de alertas.",
      );
      setAlertas([]);
    } finally {
      setCargandoAlertas(false);
    }
  }, []);

  useEffect(() => {
    void fetchAlertas();
  }, [fetchAlertas]);

  return (
    <div className="space-y-8 animate-in fade-in slide-in-from-bottom-4 duration-500 pb-10">
      <header className="relative overflow-hidden rounded-[2rem] bg-gradient-to-r from-indigo-700 via-violet-700 to-fuchsia-700 p-8 shadow-[0_30px_70px_rgba(67,56,202,0.18)] text-white">
        <div className="absolute inset-x-0 top-0 h-48 bg-white/10 blur-3xl" />
        <div className="absolute top-10 right-10 w-32 h-32 rounded-full bg-white/10 blur-2xl" />
        <div className="absolute bottom-0 left-0 w-40 h-40 rounded-full bg-white/5 blur-2xl" />
        <div className="relative z-10 flex flex-col gap-6">
          <div>
            <h1 className="text-4xl sm:text-5xl font-black tracking-tight">
              Hola, {saludo}
            </h1>
            <p className="mt-2 text-lg sm:text-xl text-indigo-100 font-medium">
              ¿Hacia dónde te diriges hoy?
            </p>
          </div>

          <div className="flex flex-col sm:flex-row gap-4 max-w-2xl">
            <Link
              to="/rutas"
              className="flex-1 rounded-3xl bg-white px-6 py-4 text-indigo-700 font-bold shadow-lg shadow-indigo-900/10 transition-all hover:-translate-y-0.5 hover:bg-slate-50"
            >
              <div className="flex items-center justify-center gap-2">
                <Navigation className="w-5 h-5" /> Buscar Ruta Segura
              </div>
            </Link>
            <Link
              to="/mapa"
              className="flex-1 rounded-3xl bg-indigo-900/90 border border-white/20 px-6 py-4 text-white font-bold shadow-lg shadow-indigo-950/10 transition-all hover:-translate-y-0.5 hover:bg-indigo-800"
            >
              <div className="flex items-center justify-center gap-2">
                <MapPin className="w-5 h-5" /> Ver Mapa
              </div>
            </Link>
          </div>
        </div>
      </header>

      <section className="grid gap-6 lg:grid-cols-[1.2fr_0.8fr]">
        <div className="bg-white rounded-[2rem] border border-slate-200 shadow-sm p-6">
          <div className="flex items-center justify-between gap-4 mb-6">
            <div>
              <h2 className="text-xl font-black text-slate-900">
                Alertas Recientes
              </h2>
              <p className="text-sm text-slate-500 mt-1">
                Incidencias activas cerca de tu ruta.
              </p>
            </div>
            <Link
              to="/mapa"
              className="text-sm font-bold text-indigo-600 hover:text-indigo-700"
            >
              Ver en mapa
            </Link>
          </div>

          {cargandoAlertas ? (
            <div className="flex items-center justify-center gap-2 py-10 text-slate-500 text-sm">
              <Loader2 className="w-5 h-5 animate-spin" />
              Cargando alertas reales del servidor…
            </div>
          ) : errorAlertas ? (
            <p className="text-sm text-amber-800 bg-amber-50 border border-amber-200 rounded-2xl px-4 py-3">
              {errorAlertas}
            </p>
          ) : alertas.length === 0 ? (
            <div className="rounded-3xl border border-slate-200 bg-slate-50 p-6 text-center">
              <p className="text-sm text-slate-600 font-medium">
                Aún no hay reportes recientes en el área, o el servidor no
                devolvió datos. Cuando tú o otros usuarios reporten, aparecerán
                aquí.
              </p>
              <Link
                to="/reportar"
                className="mt-3 inline-block text-sm font-bold text-indigo-600 hover:underline"
              >
                Hacer un reporte
              </Link>
            </div>
          ) : (
            <div className="space-y-4">
              {alertas.map((a) => {
                const tipo = String(a.tipoIncidente || "Otro");
                const ubicacion = String(
                  a.ubicacion || "Ubicación desconocida",
                );
                const st = classAlerta(tipo);
                const titulo = `${tipo}: ${truncar(ubicacion, 64)}`;
                const cuerpo = a.descripcion?.trim()
                  ? a.descripcion.trim()
                  : ubicacion;
                const Icono = usarIconoEscudo(tipo)
                  ? ShieldAlert
                  : AlertTriangle;
                return (
                  <div
                    key={a.id}
                    className={`rounded-3xl p-4 shadow-sm ${st.box}`}
                  >
                    <div className="flex gap-4 items-start">
                      <div
                        className={`shrink-0 rounded-3xl p-3 text-white shadow-lg ${st.iconBg}`}
                      >
                        <Icono className="w-5 h-5" />
                      </div>
                      <div className="flex-1 min-w-0">
                        <h3
                          className={`text-base font-bold leading-snug ${st.title}`}
                        >
                          {truncar(titulo, 120)}
                        </h3>
                        <p
                          className={`mt-2 text-sm leading-6 line-clamp-3 ${st.body}`}
                        >
                          {cuerpo}
                        </p>
                        <span
                          className={`mt-3 inline-block text-xs font-bold ${st.time}`}
                        >
                          {textoRelativo(a.fechaReporte)}
                        </span>
                      </div>
                    </div>
                  </div>
                );
              })}
            </div>
          )}
        </div>

        <div className="bg-white rounded-[2rem] border border-slate-200 shadow-sm p-6">
          <div className="mb-5">
            <h2 className="text-xl font-black text-slate-900">
              Ir a un Lugar Frecuente
            </h2>
            <p className="text-sm text-slate-500 mt-1">
              Accede rápido a tus destinos guardados.
            </p>
          </div>

          <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
            {[
              { name: "Casa", icon: "🏠", time: "15 min" },
              { name: "Universidad", icon: "🎓", time: "22 min" },
              { name: "Trabajo", icon: "💼", time: "35 min" },
              { name: "Paradero Sur", icon: "🚏", time: "5 min" },
            ].map((lugar, i) => (
              <Link
                to="/rutas"
                key={i}
                className="group rounded-[1.75rem] border border-slate-200 bg-slate-50 px-4 py-5 text-center transition hover:border-indigo-300 hover:shadow-lg"
              >
                <div className="mx-auto mb-4 flex h-14 w-14 items-center justify-center rounded-full bg-white text-3xl shadow-sm transition group-hover:bg-indigo-50">
                  {lugar.icon}
                </div>
                <h3 className="text-base font-bold text-slate-900">
                  {lugar.name}
                </h3>
                <p className="mt-2 text-xs text-slate-500 font-medium flex items-center justify-center gap-1">
                  <Clock className="w-3 h-3" /> {lugar.time}
                </p>
              </Link>
            ))}
          </div>
        </div>
      </section>
    </div>
  );
}

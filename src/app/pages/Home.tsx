import { Navigation, MapPin, AlertTriangle, Clock, ShieldAlert, ChevronRight } from "lucide-react";
import { Link } from "react-router";

export function Home() {
  return (
    <div className="space-y-8 animate-in fade-in slide-in-from-bottom-4 duration-500 pb-10">
      <header className="bg-indigo-600 rounded-3xl p-6 text-white shadow-lg relative overflow-hidden">
        <div className="absolute top-0 right-0 w-64 h-64 bg-white opacity-5 rounded-full -translate-y-1/2 translate-x-1/3"></div>
        <div className="relative z-10">
          <h1 className="text-3xl font-black mb-2">Hola, Andrea</h1>
          <p className="text-indigo-100 font-medium text-lg">¿Hacia dónde te diriges hoy?</p>
          
          <div className="mt-6 flex flex-col sm:flex-row gap-3">
            <Link to="/rutas" className="bg-white text-indigo-700 font-bold px-6 py-3.5 rounded-xl hover:bg-slate-50 transition-colors flex items-center justify-center gap-2 shadow-sm">
              <Navigation className="w-5 h-5" /> Buscar Ruta Segura
            </Link>
            <Link to="/mapa" className="bg-indigo-800 text-white font-bold px-6 py-3.5 rounded-xl hover:bg-indigo-900 transition-colors flex items-center justify-center gap-2 border border-indigo-500">
              <MapPin className="w-5 h-5" /> Ver Mapa
            </Link>
          </div>
        </div>
      </header>

      {/* Alertas y Notificaciones (Pantalla 20 integrada en Home) */}
      <div>
        <div className="flex items-center justify-between mb-4">
          <h2 className="text-xl font-black text-slate-900">Alertas Recientes</h2>
          <span className="text-sm font-bold text-indigo-600 cursor-pointer">Ver todas</span>
        </div>
        <div className="bg-white rounded-3xl border border-slate-200 shadow-sm p-4 space-y-3">
          <div className="flex gap-4 p-3 bg-red-50 rounded-2xl border border-red-100">
            <div className="bg-red-500 text-white p-2 rounded-xl h-fit">
              <ShieldAlert className="w-5 h-5" />
            </div>
            <div>
              <h3 className="font-bold text-red-900">Precaución: Zona Universitaria</h3>
              <p className="text-sm text-red-700 font-medium mt-1">2 reportes recientes de robos en las últimas 3 horas cerca de la puerta norte.</p>
              <span className="text-xs text-red-500 font-bold mt-2 block">Hace 15 min</span>
            </div>
          </div>
          
          <div className="flex gap-4 p-3 bg-amber-50 rounded-2xl border border-amber-100">
            <div className="bg-amber-500 text-white p-2 rounded-xl h-fit">
              <AlertTriangle className="w-5 h-5" />
            </div>
            <div>
              <h3 className="font-bold text-amber-900">Vía en mal estado</h3>
              <p className="text-sm text-amber-700 font-medium mt-1">Se reportó falta de iluminación en la Ciclovía Arequipa cuadra 5.</p>
              <span className="text-xs text-amber-500 font-bold mt-2 block">Hace 2 horas</span>
            </div>
          </div>
        </div>
      </div>

      {/* Lugares Frecuentes Rápido */}
      <div>
        <h2 className="text-xl font-black text-slate-900 mb-4">Ir a un Lugar Frecuente</h2>
        <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
          {[
            { name: "Casa", icon: "🏠", time: "15 min" },
            { name: "Universidad", icon: "🎓", time: "22 min" },
            { name: "Trabajo", icon: "💼", time: "35 min" },
            { name: "Paradero Sur", icon: "🚏", time: "5 min" },
          ].map((lugar, i) => (
            <Link to="/rutas" key={i} className="bg-white p-4 rounded-2xl border border-slate-200 shadow-sm hover:border-indigo-300 hover:shadow-md transition-all group flex flex-col items-center text-center gap-2">
              <div className="w-12 h-12 bg-slate-100 rounded-full flex items-center justify-center text-2xl group-hover:bg-indigo-50">
                {lugar.icon}
              </div>
              <div>
                <h3 className="font-bold text-slate-900">{lugar.name}</h3>
                <p className="text-xs text-slate-500 font-medium flex items-center justify-center gap-1">
                  <Clock className="w-3 h-3" /> {lugar.time}
                </p>
              </div>
            </Link>
          ))}
        </div>
      </div>
    </div>
  );
}
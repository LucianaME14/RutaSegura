import { Settings, Phone, MapPin, FileText, ChevronRight, LogOut, HeartPulse, History, ShieldAlert, Navigation } from "lucide-react";
import { useState } from "react";
import { useNavigate } from "react-router";

export function Perfil() {
  const navigate = useNavigate();
  // Estado para manejar las sub-vistas del perfil (Configuración, Mis Reportes, Historial)
  const [activeTab, setActiveTab] = useState<"principal" | "reportes" | "historial" | "configuracion">("principal");

  if (activeTab === "reportes") {
    return (
      <div className="max-w-3xl mx-auto space-y-6 animate-in fade-in slide-in-from-right-4 duration-300">
        <button onClick={() => setActiveTab("principal")} className="flex items-center gap-2 text-indigo-600 font-bold mb-4 hover:bg-indigo-50 px-3 py-1.5 rounded-lg w-fit transition-colors">
          <ChevronRight className="w-4 h-4 rotate-180" /> Volver al Perfil
        </button>
        <h1 className="text-2xl font-black text-slate-900">Mis Reportes</h1>
        <p className="text-slate-500 font-medium">Historial de incidencias que has reportado a la comunidad.</p>
        
        <div className="space-y-4">
          {[
            { tipo: "Falta iluminación", zona: "Calle Los Pinos", fecha: "Ayer", estado: "Validado", color: "text-emerald-600 bg-emerald-50 border-emerald-200" },
            { tipo: "Hueco en vía", zona: "Av. Principal cdra 4", fecha: "Hace 1 semana", estado: "En Revisión", color: "text-amber-600 bg-amber-50 border-amber-200" },
            { tipo: "Robo", zona: "Parque Sur", fecha: "Hace 1 mes", estado: "Resuelto", color: "text-blue-600 bg-blue-50 border-blue-200" },
          ].map((rep, i) => (
            <div key={i} className="bg-white p-4 rounded-2xl border border-slate-200 shadow-sm flex items-center justify-between">
              <div className="flex items-center gap-4">
                <div className="bg-slate-100 p-3 rounded-xl">
                  <ShieldAlert className="w-6 h-6 text-slate-600" />
                </div>
                <div>
                  <h3 className="font-bold text-slate-900">{rep.tipo}</h3>
                  <p className="text-sm text-slate-500 font-medium">{rep.zona} • {rep.fecha}</p>
                </div>
              </div>
              <span className={`px-3 py-1 text-xs font-bold rounded-lg border ${rep.color}`}>
                {rep.estado}
              </span>
            </div>
          ))}
        </div>
      </div>
    );
  }

  if (activeTab === "historial") {
    return (
      <div className="max-w-3xl mx-auto space-y-6 animate-in fade-in slide-in-from-right-4 duration-300">
        <button onClick={() => setActiveTab("principal")} className="flex items-center gap-2 text-indigo-600 font-bold mb-4 hover:bg-indigo-50 px-3 py-1.5 rounded-lg w-fit transition-colors">
          <ChevronRight className="w-4 h-4 rotate-180" /> Volver al Perfil
        </button>
        <h1 className="text-2xl font-black text-slate-900">Historial de Rutas</h1>
        
        <div className="space-y-3">
          {[
            { origen: "Universidad", destino: "Casa", fecha: "Hoy, 18:30", modo: "Caminando" },
            { origen: "Casa", destino: "Trabajo", fecha: "Hoy, 08:15", modo: "Bicicleta" },
            { origen: "Trabajo", destino: "Universidad", fecha: "Ayer, 16:00", modo: "Caminando" },
          ].map((ruta, i) => (
            <div key={i} className="bg-white p-4 rounded-2xl border border-slate-200 shadow-sm flex justify-between items-center group cursor-pointer hover:border-indigo-300 transition-colors">
              <div>
                <div className="flex items-center gap-2 font-bold text-slate-900">
                  <span>{ruta.origen}</span>
                  <Navigation className="w-3 h-3 text-slate-400 rotate-90" />
                  <span>{ruta.destino}</span>
                </div>
                <p className="text-xs text-slate-500 font-medium mt-1">{ruta.fecha} • {ruta.modo}</p>
              </div>
              <button className="text-indigo-600 text-sm font-bold bg-indigo-50 px-3 py-1.5 rounded-lg opacity-0 group-hover:opacity-100 transition-opacity">
                Repetir ruta
              </button>
            </div>
          ))}
        </div>
      </div>
    );
  }

  if (activeTab === "configuracion") {
    return (
      <div className="max-w-3xl mx-auto space-y-6 animate-in fade-in slide-in-from-right-4 duration-300 pb-10">
        <button onClick={() => setActiveTab("principal")} className="flex items-center gap-2 text-indigo-600 font-bold mb-4 hover:bg-indigo-50 px-3 py-1.5 rounded-lg w-fit transition-colors">
          <ChevronRight className="w-4 h-4 rotate-180" /> Volver al Perfil
        </button>
        <h1 className="text-2xl font-black text-slate-900">Configuración del Sistema</h1>
        
        <div className="bg-white rounded-3xl border border-slate-200 shadow-sm overflow-hidden divide-y divide-slate-100">
          <div className="p-5">
            <h3 className="font-bold text-slate-900 mb-4">Preferencias de Ruta</h3>
            <div className="space-y-4">
              <label className="flex items-center justify-between">
                <span className="text-sm font-medium text-slate-700">Evitar zonas oscuras de noche</span>
                <input type="checkbox" defaultChecked className="toggle-checkbox w-10 h-5 rounded-full bg-slate-200 appearance-none cursor-pointer border-2 border-transparent checked:bg-indigo-600 transition-colors" />
              </label>
              <label className="flex items-center justify-between">
                <span className="text-sm font-medium text-slate-700">Modo de movilidad predeterminado</span>
                <select className="bg-slate-50 border border-slate-200 rounded-lg text-sm px-2 py-1 font-bold text-slate-700">
                  <option>Peatón</option>
                  <option>Bicicleta</option>
                </select>
              </label>
            </div>
          </div>
          
          <div className="p-5">
            <h3 className="font-bold text-slate-900 mb-4">Notificaciones y Alertas</h3>
            <div className="space-y-4">
              <label className="flex items-center justify-between">
                <div>
                  <span className="text-sm font-medium text-slate-700 block">Alertas de riesgo en tiempo real</span>
                  <span className="text-xs text-slate-500">Notificarme si entro a una zona peligrosa</span>
                </div>
                <input type="checkbox" defaultChecked className="toggle-checkbox w-10 h-5 rounded-full bg-slate-200 appearance-none cursor-pointer border-2 border-transparent checked:bg-indigo-600 transition-colors" />
              </label>
              <label className="flex items-center justify-between">
                <div>
                  <span className="text-sm font-medium text-slate-700 block">Aviso automático de llegada</span>
                  <span className="text-xs text-slate-500">Alertar a contactos si no marco "Llegué bien"</span>
                </div>
                <input type="checkbox" defaultChecked className="toggle-checkbox w-10 h-5 rounded-full bg-slate-200 appearance-none cursor-pointer border-2 border-transparent checked:bg-indigo-600 transition-colors" />
              </label>
            </div>
          </div>
        </div>
      </div>
    );
  }

  // Vista Principal de Perfil
  return (
    <div className="max-w-3xl mx-auto space-y-8 animate-in fade-in slide-in-from-bottom-4 duration-500 pb-10">
      <header className="flex items-center gap-6">
        <div className="w-24 h-24 rounded-full bg-indigo-100 flex items-center justify-center text-indigo-600 text-3xl font-black border-4 border-white shadow-md relative">
          AL
          <div className="absolute bottom-0 right-0 w-6 h-6 bg-emerald-500 border-2 border-white rounded-full"></div>
        </div>
        <div>
          <h1 className="text-3xl font-black text-slate-900">Andrea López</h1>
          <p className="text-slate-500 font-medium">andrea.lopez@universidad.edu</p>
          <div className="mt-2 inline-flex items-center gap-1.5 bg-emerald-100 text-emerald-700 px-3 py-1 rounded-lg text-xs font-bold">
            <HeartPulse className="w-3.5 h-3.5" /> Cuenta Protegida
          </div>
        </div>
      </header>

      <div className="grid md:grid-cols-2 gap-6">
        {/* Contactos de Emergencia */}
        <div className="bg-white rounded-3xl border border-slate-200 shadow-sm p-6">
          <div className="flex items-center justify-between mb-6">
            <h2 className="text-xl font-bold text-slate-900 flex items-center gap-2">
              <Phone className="w-5 h-5 text-indigo-600" />
              Contactos SOS
            </h2>
            <button className="text-indigo-600 text-sm font-bold hover:underline">Añadir</button>
          </div>
          
          <div className="space-y-4">
            <div className="flex items-center justify-between p-4 bg-slate-50 rounded-2xl border border-slate-100">
              <div>
                <p className="font-bold text-slate-900">Mamá</p>
                <p className="text-slate-500 text-sm font-medium">+51 987 654 321</p>
              </div>
              <div className="flex gap-2">
                <span className="bg-red-100 text-red-700 text-xs font-bold px-2 py-1 rounded-md">SOS 1</span>
              </div>
            </div>
            
            <div className="flex items-center justify-between p-4 bg-slate-50 rounded-2xl border border-slate-100">
              <div>
                <p className="font-bold text-slate-900">Carlos (Hermano)</p>
                <p className="text-slate-500 text-sm font-medium">+51 912 345 678</p>
              </div>
              <div className="flex gap-2">
                <span className="bg-amber-100 text-amber-700 text-xs font-bold px-2 py-1 rounded-md">SOS 2</span>
              </div>
            </div>
          </div>
        </div>

        {/* Lugares Frecuentes */}
        <div className="bg-white rounded-3xl border border-slate-200 shadow-sm p-6">
          <div className="flex items-center justify-between mb-6">
            <h2 className="text-xl font-bold text-slate-900 flex items-center gap-2">
              <MapPin className="w-5 h-5 text-indigo-600" />
              Ubicaciones Guardadas
            </h2>
            <button className="text-indigo-600 text-sm font-bold hover:underline">Añadir</button>
          </div>
          
          <div className="space-y-3">
            {[
              { label: "Casa", address: "Av. Los Rosales 123", icon: "🏠" },
              { label: "Universidad", address: "Campus Norte, Pabellón C", icon: "🎓" },
              { label: "Trabajo", address: "Torre Empresarial, Piso 4", icon: "💼" },
            ].map((place, i) => (
              <div key={i} className="flex items-center gap-4 p-3 hover:bg-slate-50 rounded-xl transition-colors cursor-pointer group">
                <div className="w-10 h-10 bg-slate-100 rounded-full flex items-center justify-center text-lg">
                  {place.icon}
                </div>
                <div className="flex-1">
                  <p className="font-bold text-slate-900">{place.label}</p>
                  <p className="text-slate-500 text-xs font-medium truncate pr-4">{place.address}</p>
                </div>
                <ChevronRight className="w-4 h-4 text-slate-300 group-hover:text-indigo-500" />
              </div>
            ))}
          </div>
        </div>
      </div>

      {/* Otras Opciones */}
      <div className="bg-white rounded-3xl border border-slate-200 shadow-sm overflow-hidden">
        <div className="divide-y divide-slate-100">
          
          <button onClick={() => setActiveTab("historial")} className="w-full flex items-center justify-between p-5 hover:bg-slate-50 transition-colors">
            <div className="flex items-center gap-3">
              <div className="p-2 bg-indigo-50 text-indigo-600 rounded-lg"><History className="w-5 h-5" /></div>
              <span className="font-bold text-slate-700">Historial de Rutas</span>
            </div>
            <ChevronRight className="w-5 h-5 text-slate-400" />
          </button>

          <button onClick={() => setActiveTab("reportes")} className="w-full flex items-center justify-between p-5 hover:bg-slate-50 transition-colors">
            <div className="flex items-center gap-3">
              <div className="p-2 bg-indigo-50 text-indigo-600 rounded-lg"><FileText className="w-5 h-5" /></div>
              <span className="font-bold text-slate-700">Mis Reportes</span>
            </div>
            <ChevronRight className="w-5 h-5 text-slate-400" />
          </button>

          <button onClick={() => setActiveTab("configuracion")} className="w-full flex items-center justify-between p-5 hover:bg-slate-50 transition-colors">
            <div className="flex items-center gap-3">
              <div className="p-2 bg-indigo-50 text-indigo-600 rounded-lg"><Settings className="w-5 h-5" /></div>
              <span className="font-bold text-slate-700">Configuración</span>
            </div>
            <ChevronRight className="w-5 h-5 text-slate-400" />
          </button>
          
          <button onClick={() => navigate("/")} className="w-full flex items-center justify-between p-5 hover:bg-red-50 transition-colors text-red-600">
            <div className="flex items-center gap-3">
              <div className="p-2 bg-red-50 text-red-600 rounded-lg"><LogOut className="w-5 h-5" /></div>
              <span className="font-bold">Cerrar Sesión</span>
            </div>
          </button>
        </div>
      </div>
    </div>
  );
}
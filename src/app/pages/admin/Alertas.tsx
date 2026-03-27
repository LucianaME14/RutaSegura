import { BellRing, Plus, Settings2, Trash2 } from "lucide-react";

export function AdminAlertas() {
  return (
    <div className="space-y-6 animate-in fade-in duration-500">
      <header className="flex justify-between items-end">
        <div>
          <h1 className="text-3xl font-black text-slate-900">Gestión de Alertas</h1>
          <p className="text-slate-500 mt-2 font-medium">Configura y emite alertas globales para los usuarios de Ruta Segura.</p>
        </div>
        <button className="bg-indigo-600 text-white font-bold px-5 py-2.5 rounded-xl hover:bg-indigo-700 flex items-center gap-2 shadow-sm">
          <Plus className="w-4 h-4" /> Crear Alerta Manual
        </button>
      </header>

      <div className="grid lg:grid-cols-3 gap-6">
        {/* Columna Izquierda: Alertas Activas */}
        <div className="lg:col-span-2 space-y-4">
          <h2 className="text-lg font-bold text-slate-900 flex items-center gap-2">
            <BellRing className="w-5 h-5 text-amber-500" /> Alertas Activas (Broadcast)
          </h2>
          
          <div className="bg-white rounded-3xl border border-slate-200 shadow-sm p-2">
            {[
              { titulo: "Cierre de Av. Universitaria por obras", tipo: "Informativa", alcance: "Global", fecha: "Vence en 2 días" },
              { titulo: "Alta incidencia de robos reportada", tipo: "Peligro", alcance: "Zona Norte", fecha: "Automática por ML" },
            ].map((alerta, i) => (
              <div key={i} className="flex justify-between items-center p-4 hover:bg-slate-50 rounded-2xl border-b border-slate-100 last:border-0 transition-colors">
                <div>
                  <div className="flex items-center gap-2 mb-1">
                    <span className={`text-[10px] uppercase font-black px-2 py-0.5 rounded-md ${alerta.tipo === 'Peligro' ? 'bg-red-100 text-red-700' : 'bg-blue-100 text-blue-700'}`}>
                      {alerta.tipo}
                    </span>
                    <span className="text-[10px] font-bold text-slate-500 bg-slate-100 px-2 py-0.5 rounded-md">
                      Alcance: {alerta.alcance}
                    </span>
                  </div>
                  <h3 className="font-bold text-slate-900">{alerta.titulo}</h3>
                  <p className="text-xs text-slate-500 font-medium mt-1">{alerta.fecha}</p>
                </div>
                <button className="text-slate-400 hover:text-red-500 p-2">
                  <Trash2 className="w-5 h-5" />
                </button>
              </div>
            ))}
          </div>
        </div>

        {/* Columna Derecha: Configuración Automática */}
        <div className="space-y-4">
          <h2 className="text-lg font-bold text-slate-900 flex items-center gap-2">
            <Settings2 className="w-5 h-5 text-indigo-500" /> Reglas del Sistema (IA)
          </h2>
          
          <div className="bg-white rounded-3xl border border-slate-200 shadow-sm p-5 space-y-5">
            <div>
              <div className="flex justify-between items-center mb-2">
                <span className="font-bold text-sm text-slate-700">Umbral de Reportes para Alerta</span>
                <span className="text-xs font-black text-indigo-600 bg-indigo-50 px-2 py-1 rounded">3 reportes / hora</span>
              </div>
              <input type="range" min="1" max="10" defaultValue="3" className="w-full accent-indigo-600" />
            </div>

            <hr className="border-slate-100" />

            <label className="flex items-center justify-between">
              <div>
                <span className="text-sm font-bold text-slate-900 block">Auto-Alertas por ML</span>
                <span className="text-xs text-slate-500 font-medium leading-tight block pr-4">La IA emitirá avisos a usuarios si se acercan a una zona roja.</span>
              </div>
              <input type="checkbox" defaultChecked className="toggle-checkbox w-10 h-5 rounded-full bg-slate-200 appearance-none cursor-pointer border-2 border-transparent checked:bg-indigo-600 transition-colors flex-shrink-0" />
            </label>

            <label className="flex items-center justify-between">
              <div>
                <span className="text-sm font-bold text-slate-900 block">Notificar desvíos en vivo</span>
                <span className="text-xs text-slate-500 font-medium leading-tight block pr-4">Sugerir ruta alternativa si ocurre incidencia en trayecto activo.</span>
              </div>
              <input type="checkbox" defaultChecked className="toggle-checkbox w-10 h-5 rounded-full bg-slate-200 appearance-none cursor-pointer border-2 border-transparent checked:bg-indigo-600 transition-colors flex-shrink-0" />
            </label>
          </div>
        </div>
      </div>
    </div>
  );
}
import { Settings, ShieldCheck, Database, Server, Mail, Save } from "lucide-react";

export function AdminConfiguracion() {
  return (
    <div className="space-y-6 animate-in fade-in duration-500 max-w-4xl">
      <header>
        <h1 className="text-3xl font-black text-slate-900">Configuración del Sistema</h1>
        <p className="text-slate-500 mt-2 font-medium">Ajustes globales de la plataforma Ruta Segura y parámetros técnicos.</p>
      </header>

      <div className="grid md:grid-cols-2 gap-6">
        {/* Parámetros de Seguridad y ML */}
        <div className="bg-white rounded-3xl border border-slate-200 shadow-sm p-6 space-y-6">
          <div className="flex items-center gap-3 border-b border-slate-100 pb-4">
            <div className="bg-indigo-50 p-2 rounded-xl text-indigo-600">
              <ShieldCheck className="w-5 h-5" />
            </div>
            <h2 className="font-bold text-lg text-slate-900">Parámetros del Algoritmo (ML)</h2>
          </div>
          
          <div className="space-y-4">
            <div>
              <label className="block text-sm font-bold text-slate-700 mb-1">Peso de zonas oscuras en Score (%)</label>
              <input type="number" defaultValue="40" className="w-full bg-slate-50 border border-slate-200 rounded-xl px-3 py-2 focus:ring-2 focus:ring-indigo-500" />
            </div>
            <div>
              <label className="block text-sm font-bold text-slate-700 mb-1">Tiempo caducidad de reportes menores (horas)</label>
              <input type="number" defaultValue="24" className="w-full bg-slate-50 border border-slate-200 rounded-xl px-3 py-2 focus:ring-2 focus:ring-indigo-500" />
            </div>
            <div>
              <label className="block text-sm font-bold text-slate-700 mb-1">Nivel confianza mínimo de IA para auto-aprobar</label>
              <select defaultValue="85%" className="w-full bg-slate-50 border border-slate-200 rounded-xl px-3 py-2 font-medium">
                <option value="90%">90% (Estricto)</option>
                <option value="85%">85% (Recomendado)</option>
                <option value="75%">75% (Flexible)</option>
              </select>
            </div>
          </div>
        </div>

        {/* API y Servicios */}
        <div className="bg-white rounded-3xl border border-slate-200 shadow-sm p-6 space-y-6">
          <div className="flex items-center gap-3 border-b border-slate-100 pb-4">
            <div className="bg-slate-100 p-2 rounded-xl text-slate-700">
              <Server className="w-5 h-5" />
            </div>
            <h2 className="font-bold text-lg text-slate-900">Integraciones de API</h2>
          </div>
          
          <div className="space-y-4">
            <div>
              <label className="block text-sm font-bold text-slate-700 mb-1">Google Maps API Key</label>
              <input type="password" defaultValue="AIzaSyB-XXXXXXXXXXXXXXXXXXXXX" className="w-full bg-slate-50 border border-slate-200 rounded-xl px-3 py-2 text-slate-500 focus:ring-2 focus:ring-indigo-500" />
            </div>
            <div>
              <label className="block text-sm font-bold text-slate-700 mb-1">Servidor de Notificaciones (Push)</label>
              <input type="text" defaultValue="https://push.rutasegura.net" className="w-full bg-slate-50 border border-slate-200 rounded-xl px-3 py-2 text-slate-700 focus:ring-2 focus:ring-indigo-500" />
            </div>
            <div className="pt-2">
              <button className="w-full bg-slate-900 hover:bg-slate-800 text-white font-bold py-3 rounded-xl transition-colors flex justify-center items-center gap-2">
                <Database className="w-4 h-4" /> Probar Conexión a Base de Datos
              </button>
            </div>
          </div>
        </div>
      </div>
      
      <div className="flex justify-end pt-4">
        <button className="bg-indigo-600 hover:bg-indigo-700 text-white font-black py-3 px-8 rounded-xl transition-all shadow-md flex items-center gap-2">
          <Save className="w-5 h-5" /> Guardar Cambios Globales
        </button>
      </div>
    </div>
  );
}
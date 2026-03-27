import { Users, AlertTriangle, Route, Activity, ArrowUpRight } from "lucide-react";
import { AreaChart, Area, XAxis, Tooltip, ResponsiveContainer } from "recharts";

const data = [
  { name: "Lun", reportes: 24 },
  { name: "Mar", reportes: 35 },
  { name: "Mié", reportes: 18 },
  { name: "Jue", reportes: 42 },
  { name: "Vie", reportes: 55 },
  { name: "Sáb", reportes: 68 },
  { name: "Dom", reportes: 40 },
];

export function AdminDashboard() {
  return (
    <div className="space-y-8 animate-in fade-in slide-in-from-bottom-4 duration-500">
      <header>
        <h1 className="text-3xl font-black text-slate-900">Dashboard Administrativo</h1>
        <p className="text-slate-500 mt-2 font-medium">Resumen general del sistema Ruta Segura (Vista ML y Analítica).</p>
      </header>

      {/* KPIs */}
      <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
        {[
          { label: "Usuarios Activos", value: "1,248", icon: Users, color: "text-blue-600", bg: "bg-blue-100", trend: "+12%" },
          { label: "Reportes Pendientes", value: "34", icon: AlertTriangle, color: "text-red-600", bg: "bg-red-100", trend: "-5%" },
          { label: "Rutas Consultadas", value: "8.5k", icon: Route, color: "text-emerald-600", bg: "bg-emerald-100", trend: "+24%" },
          { label: "Alertas Emitidas", value: "156", icon: Activity, color: "text-amber-600", bg: "bg-amber-100", trend: "+2%" },
        ].map((stat) => (
          <div key={stat.label} className="bg-white p-5 rounded-3xl border border-slate-200 shadow-sm">
            <div className="flex justify-between items-start mb-4">
              <div className={`p-3 rounded-xl ${stat.bg}`}>
                <stat.icon className={`w-6 h-6 ${stat.color}`} />
              </div>
              <span className={`text-xs font-bold flex items-center ${stat.trend.startsWith('+') ? 'text-emerald-600' : 'text-red-600'}`}>
                {stat.trend} <ArrowUpRight className="w-3 h-3 ml-1" />
              </span>
            </div>
            <div>
              <h3 className="text-3xl font-black text-slate-900">{stat.value}</h3>
              <p className="text-slate-500 text-sm font-semibold mt-1">{stat.label}</p>
            </div>
          </div>
        ))}
      </div>

      <div className="grid md:grid-cols-3 gap-8">
        {/* Gráfico Analítico */}
        <div className="md:col-span-2 bg-white p-6 rounded-3xl border border-slate-200 shadow-sm">
          <div className="flex justify-between items-center mb-6">
            <h2 className="text-xl font-bold text-slate-900">Volumen de Reportes (Última Semana)</h2>
            <select className="bg-slate-50 border-none text-sm text-slate-700 font-bold rounded-xl px-3 py-2">
              <option>Últimos 7 días</option>
              <option>Este mes</option>
            </select>
          </div>
          <div className="h-72 w-full" style={{ minWidth: 0 }}>
            <ResponsiveContainer width="100%" height={288}>
              <AreaChart data={data} key="dashboard-area-chart">
                <defs key="defs">
                  <linearGradient id="colorReportes" x1="0" y1="0" x2="0" y2="1">
                    <stop offset="5%" stopColor="#4f46e5" stopOpacity={0.3}/>
                    <stop offset="95%" stopColor="#4f46e5" stopOpacity={0}/>
                  </linearGradient>
                </defs>
                <XAxis key="xaxis" dataKey="name" axisLine={false} tickLine={false} tick={{fill: '#64748b', fontSize: 12, fontWeight: 'bold'}} />
                <Tooltip key="tooltip"
                  contentStyle={{ borderRadius: '16px', border: '1px solid #e2e8f0', boxShadow: '0 4px 6px -1px rgb(0 0 0 / 0.1)', fontWeight: 'bold' }}
                />
                <Area key="area" type="monotone" dataKey="reportes" stroke="#4f46e5" strokeWidth={4} fillOpacity={1} fill="url(#colorReportes)" />
              </AreaChart>
            </ResponsiveContainer>
          </div>
        </div>

        {/* Zonas Críticas Predichas por ML */}
        <div className="bg-slate-900 rounded-3xl p-6 text-white shadow-xl flex flex-col">
          <div className="flex items-center gap-3 mb-6">
            <div className="p-2 bg-indigo-500/20 rounded-lg text-indigo-400">
              <Activity className="w-5 h-5" />
            </div>
            <h2 className="text-xl font-bold">Predicción ML (Riesgo)</h2>
          </div>
          
          <div className="space-y-4 flex-1">
            {[
              { zona: "Av. Universitaria Sur", prob: "85%", hora: "18:00 - 21:00", color: "text-red-400", bg: "bg-red-400" },
              { zona: "Parque Kennedy", prob: "72%", hora: "20:00 - 23:00", color: "text-orange-400", bg: "bg-orange-400" },
              { zona: "Cruce Calle 8", prob: "60%", hora: "Todo el día", color: "text-amber-400", bg: "bg-amber-400" },
            ].map((pred) => (
              <div key={pred.zona} className="bg-white/10 p-4 rounded-2xl border border-white/5">
                <div className="flex justify-between items-start mb-2">
                  <span className="font-bold text-slate-100">{pred.zona}</span>
                  <span className={`font-black ${pred.color}`}>{pred.prob}</span>
                </div>
                <div className="w-full bg-slate-800 rounded-full h-1.5 mb-2">
                  <div className={`${pred.bg} h-1.5 rounded-full`} style={{ width: pred.prob }}></div>
                </div>
                <span className="text-xs text-slate-400 font-medium">Horario crítico: {pred.hora}</span>
              </div>
            ))}
          </div>
          
          <button className="mt-6 w-full bg-indigo-600 hover:bg-indigo-500 text-white font-bold py-3 rounded-xl transition-colors">
            Generar Alertas Preventivas
          </button>
        </div>
      </div>
    </div>
  );
}
import { CheckCircle2, XCircle, AlertTriangle, Filter, MoreHorizontal } from "lucide-react";
import { useState } from "react";

const mockReportes = [
  { id: "REP-001", user: "Carlos M.", tipo: "Hueco en vía", zona: "Av. Principal", fecha: "Hace 10 min", estado: "Pendiente", IA_score: "98% Confiable" },
  { id: "REP-002", user: "Anónimo", tipo: "Robo / Peligro", zona: "Parque Sur", fecha: "Hace 25 min", estado: "Pendiente", IA_score: "85% Confiable" },
  { id: "REP-003", user: "Andrea L.", tipo: "Falta iluminación", zona: "Calle Los Pinos", fecha: "Hace 1 hr", estado: "Aprobado", IA_score: "99% Confiable" },
  { id: "REP-004", user: "Luis T.", tipo: "Basura", zona: "Facultad Ing.", fecha: "Hace 3 hrs", estado: "Rechazado", IA_score: "40% Confiable (Falso Positivo)" },
];

export function AdminReportes() {
  const [reportes, setReportes] = useState(mockReportes);

  const handleAction = (id: string, newStatus: string) => {
    setReportes(reportes.map(r => r.id === id ? { ...r, estado: newStatus } : r));
  };

  return (
    <div className="space-y-6 animate-in fade-in duration-500">
      <header className="flex flex-col md:flex-row md:items-center justify-between gap-4">
        <div>
          <h1 className="text-3xl font-black text-slate-900">Gestión de Reportes</h1>
          <p className="text-slate-500 mt-2 font-medium">Valida y modera las incidencias reportadas por los usuarios.</p>
        </div>
        <button className="flex items-center gap-2 bg-white border border-slate-200 text-slate-700 px-4 py-2.5 rounded-xl font-bold hover:bg-slate-50 shadow-sm">
          <Filter className="w-4 h-4" /> Filtros
        </button>
      </header>

      <div className="bg-white rounded-3xl border border-slate-200 shadow-sm overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full text-left border-collapse">
            <thead>
              <tr className="bg-slate-50 border-b border-slate-200 text-slate-500 text-sm font-bold uppercase tracking-wider">
                <th className="p-5">ID Reporte</th>
                <th className="p-5">Usuario / Tipo</th>
                <th className="p-5">Ubicación</th>
                <th className="p-5">Análisis IA</th>
                <th className="p-5">Estado</th>
                <th className="p-5 text-right">Acciones</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-100">
              {reportes.map((rep) => (
                <tr key={rep.id} className="hover:bg-slate-50/50 transition-colors">
                  <td className="p-5 text-sm font-bold text-slate-900">{rep.id}</td>
                  <td className="p-5">
                    <p className="font-bold text-slate-900">{rep.tipo}</p>
                    <p className="text-xs font-medium text-slate-500">Por: {rep.user}</p>
                  </td>
                  <td className="p-5">
                    <p className="font-medium text-slate-700 text-sm">{rep.zona}</p>
                    <p className="text-xs text-slate-400">{rep.fecha}</p>
                  </td>
                  <td className="p-5">
                    <span className={`inline-flex text-xs font-bold px-2.5 py-1 rounded-lg ${rep.IA_score.includes('Falso') ? 'bg-red-100 text-red-700' : 'bg-indigo-100 text-indigo-700'}`}>
                      {rep.IA_score}
                    </span>
                  </td>
                  <td className="p-5">
                    <span className={`inline-flex items-center gap-1 text-xs font-bold px-3 py-1.5 rounded-full border ${
                      rep.estado === 'Aprobado' ? 'bg-emerald-50 text-emerald-700 border-emerald-200' :
                      rep.estado === 'Rechazado' ? 'bg-red-50 text-red-700 border-red-200' :
                      'bg-amber-50 text-amber-700 border-amber-200'
                    }`}>
                      {rep.estado === 'Pendiente' && <AlertTriangle className="w-3 h-3" />}
                      {rep.estado}
                    </span>
                  </td>
                  <td className="p-5 text-right flex items-center justify-end gap-2">
                    {rep.estado === 'Pendiente' ? (
                      <>
                        <button onClick={() => handleAction(rep.id, 'Aprobado')} className="p-2 text-emerald-600 hover:bg-emerald-50 rounded-lg tooltip" title="Aprobar">
                          <CheckCircle2 className="w-5 h-5" />
                        </button>
                        <button onClick={() => handleAction(rep.id, 'Rechazado')} className="p-2 text-red-600 hover:bg-red-50 rounded-lg tooltip" title="Rechazar">
                          <XCircle className="w-5 h-5" />
                        </button>
                      </>
                    ) : (
                      <button className="p-2 text-slate-400 hover:bg-slate-100 rounded-lg">
                        <MoreHorizontal className="w-5 h-5" />
                      </button>
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}
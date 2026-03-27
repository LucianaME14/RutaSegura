import { Users, Search, ShieldCheck, Ban, MoreHorizontal, UserCheck } from "lucide-react";
import { useState } from "react";

const mockUsuarios = [
  { id: "U-1042", nombre: "Andrea López", email: "andrea.l@univ.edu", rol: "Usuario", estado: "Activo", reportes: 12 },
  { id: "U-0891", nombre: "Carlos Mendoza", email: "carlos.m@univ.edu", rol: "Usuario", estado: "Suspendido", reportes: 45 },
  { id: "U-0001", nombre: "Admin Principal", email: "admin@rutasegura.com", rol: "Administrador", estado: "Activo", reportes: 0 },
  { id: "U-2305", nombre: "Luis Torres", email: "luis.t@univ.edu", rol: "Usuario", estado: "Activo", reportes: 3 },
];

export function AdminUsuarios() {
  const [usuarios] = useState(mockUsuarios);

  return (
    <div className="space-y-6 animate-in fade-in duration-500">
      <header className="flex flex-col md:flex-row md:items-center justify-between gap-4">
        <div>
          <h1 className="text-3xl font-black text-slate-900">Gestión de Usuarios</h1>
          <p className="text-slate-500 mt-2 font-medium">Administra las cuentas, roles y accesos a la plataforma.</p>
        </div>
        <div className="relative w-full md:w-72">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-slate-400" />
          <input 
            type="text" 
            placeholder="Buscar por correo o nombre..." 
            className="w-full bg-white border border-slate-200 rounded-xl pl-9 pr-4 py-2.5 text-sm font-medium focus:outline-none focus:ring-2 focus:ring-indigo-500"
          />
        </div>
      </header>

      <div className="bg-white rounded-3xl border border-slate-200 shadow-sm overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full text-left border-collapse">
            <thead>
              <tr className="bg-slate-50 border-b border-slate-200 text-slate-500 text-sm font-bold uppercase tracking-wider">
                <th className="p-5">Usuario</th>
                <th className="p-5">Rol</th>
                <th className="p-5">Reportes Creados</th>
                <th className="p-5">Estado</th>
                <th className="p-5 text-right">Acciones</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-100">
              {usuarios.map((user) => (
                <tr key={user.id} className="hover:bg-slate-50 transition-colors">
                  <td className="p-5">
                    <p className="font-bold text-slate-900">{user.nombre}</p>
                    <p className="text-xs font-medium text-slate-500">{user.email} • {user.id}</p>
                  </td>
                  <td className="p-5">
                    <span className={`inline-flex items-center gap-1 text-xs font-bold px-2.5 py-1 rounded-lg ${user.rol === 'Administrador' ? 'bg-slate-900 text-white' : 'bg-slate-100 text-slate-700'}`}>
                      {user.rol === 'Administrador' && <ShieldCheck className="w-3 h-3" />}
                      {user.rol}
                    </span>
                  </td>
                  <td className="p-5 font-medium text-slate-600">
                    {user.reportes}
                  </td>
                  <td className="p-5">
                    <span className={`inline-flex text-xs font-bold px-3 py-1.5 rounded-full border ${
                      user.estado === 'Activo' ? 'bg-emerald-50 text-emerald-700 border-emerald-200' :
                      'bg-red-50 text-red-700 border-red-200'
                    }`}>
                      {user.estado}
                    </span>
                  </td>
                  <td className="p-5 text-right">
                    <div className="flex justify-end gap-2">
                      {user.estado === 'Activo' && user.rol !== 'Administrador' ? (
                        <button className="p-2 text-red-600 hover:bg-red-50 rounded-lg tooltip" title="Suspender">
                          <Ban className="w-5 h-5" />
                        </button>
                      ) : user.estado === 'Suspendido' ? (
                        <button className="p-2 text-emerald-600 hover:bg-emerald-50 rounded-lg tooltip" title="Reactivar">
                          <UserCheck className="w-5 h-5" />
                        </button>
                      ) : null}
                      <button className="p-2 text-slate-400 hover:bg-slate-100 rounded-lg">
                        <MoreHorizontal className="w-5 h-5" />
                      </button>
                    </div>
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
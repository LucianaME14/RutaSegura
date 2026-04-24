import { Link, Outlet } from "react-router";
import { useAuth } from "../contexts/AuthContext";

export default function AdminRequireAuth() {
  const { user, isAdmin } = useAuth();

  if (!user) {
    return (
      <div className="min-h-[calc(100vh-4rem)] flex items-center justify-center px-4 py-10 bg-slate-950 text-slate-100">
        <div className="w-full max-w-xl rounded-3xl border border-slate-700/80 bg-slate-900/90 p-10 shadow-2xl shadow-black/20">
          <h1 className="text-4xl font-black text-white mb-4">
            Acceso admin requerido
          </h1>
          <p className="text-slate-300 mb-8 leading-relaxed">
            Debes iniciar sesión como administrador para acceder a esta sección.
          </p>
          <div className="flex flex-col gap-4 sm:flex-row sm:items-center">
            <Link
              to="/"
              className="inline-flex justify-center rounded-2xl bg-indigo-600 px-5 py-3 text-sm font-bold text-white shadow-lg shadow-indigo-500/20 hover:bg-indigo-500 transition"
            >
              Iniciar sesión
            </Link>
          </div>
        </div>
      </div>
    );
  }

  if (!isAdmin) {
    return (
      <div className="min-h-[calc(100vh-4rem)] flex items-center justify-center px-4 py-10 bg-slate-950 text-slate-100">
        <div className="w-full max-w-xl rounded-3xl border border-slate-700/80 bg-slate-900/90 p-10 shadow-2xl shadow-black/20">
          <h1 className="text-4xl font-black text-white mb-4">
            Permiso denegado
          </h1>
          <p className="text-slate-300 mb-8 leading-relaxed">
            Esta sección requiere privilegios de administrador. Tu cuenta no
            tiene permisos para ver el panel admin.
          </p>
          <div className="flex flex-col gap-4 sm:flex-row sm:items-center">
            <Link
              to="/home"
              className="inline-flex justify-center rounded-2xl bg-indigo-600 px-5 py-3 text-sm font-bold text-white shadow-lg shadow-indigo-500/20 hover:bg-indigo-500 transition"
            >
              Ir a inicio
            </Link>
          </div>
        </div>
      </div>
    );
  }

  return <Outlet />;
}

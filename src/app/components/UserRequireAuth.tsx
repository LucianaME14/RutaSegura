import { Link, Outlet } from "react-router";
import { useAuth } from "../contexts/AuthContext";

export default function UserRequireAuth() {
  const { user } = useAuth();

  if (!user) {
    return (
      <div className="min-h-[calc(100vh-4rem)] flex items-center justify-center px-4 py-10 bg-slate-950 text-slate-100">
        <div className="w-full max-w-xl rounded-3xl border border-slate-700/80 bg-slate-900/90 p-10 shadow-2xl shadow-black/20">
          <h1 className="text-4xl font-black text-white mb-4">
            Acceso restringido
          </h1>
          <p className="text-slate-300 mb-8 leading-relaxed">
            Debes iniciar sesión para continuar. Si ya tienes cuenta, ingresa
            para ver tu panel y tus rutas seguras.
          </p>
          <div className="flex flex-col gap-4 sm:flex-row sm:items-center">
            <Link
              to="/"
              className="inline-flex justify-center rounded-2xl bg-indigo-600 px-5 py-3 text-sm font-bold text-white shadow-lg shadow-indigo-500/20 hover:bg-indigo-500 transition"
            >
              Iniciar sesión
            </Link>
            <Link
              to="/registro"
              className="inline-flex justify-center rounded-2xl border border-slate-700 bg-slate-800 px-5 py-3 text-sm font-bold text-slate-100 hover:border-slate-600 hover:bg-slate-700 transition"
            >
              Crear cuenta
            </Link>
          </div>
        </div>
      </div>
    );
  }

  return <Outlet />;
}

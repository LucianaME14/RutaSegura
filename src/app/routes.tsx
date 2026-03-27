import { createBrowserRouter, Navigate } from "react-router";
import { Layout } from "./components/Layout";

// Auth
import { Auth } from "./pages/Auth";

// User Pages
import { Home } from "./pages/Home";
import { Mapa } from "./pages/Mapa";
import { Rutas } from "./pages/Rutas";
import { Reportar } from "./pages/Reportar";
import { Perfil } from "./pages/Perfil";

// Admin Pages
import { AdminDashboard } from "./pages/admin/Dashboard";
import { AdminReportes } from "./pages/admin/Reportes";
import { AdminUsuarios } from "./pages/admin/Usuarios";
import { AdminAlertas } from "./pages/admin/Alertas";
import { AdminConfiguracion } from "./pages/admin/Configuracion";

export const router = createBrowserRouter([
  {
    path: "/",
    Component: Layout,
    children: [
      { index: true, Component: Auth },
      { path: "home", Component: Home },
      { path: "mapa", Component: Mapa },
      { path: "rutas", Component: Rutas },
      { path: "reportar", Component: Reportar },
      { path: "perfil", Component: Perfil },
      
      // Admin Routes
      { path: "admin/dashboard", Component: AdminDashboard },
      { path: "admin/reportes", Component: AdminReportes },
      { path: "admin/usuarios", Component: AdminUsuarios },
      { path: "admin/mapa-calor", Component: Mapa }, 
      { path: "admin/alertas", Component: AdminAlertas }, 
      { path: "admin/configuracion", Component: AdminConfiguracion }, 
      { path: "*", Component: () => <Navigate to="/" replace /> },
    ],
  },
]);
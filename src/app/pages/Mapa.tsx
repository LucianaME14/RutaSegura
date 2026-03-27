import { MapPin, Search, Navigation, AlertTriangle, ShieldAlert, Navigation2, CheckCircle2, UserCircle } from "lucide-react";
import { Link } from "react-router";

export function Mapa() {
  return (
    <div className="space-y-4 animate-in fade-in duration-500 h-full flex flex-col">
      <header className="flex flex-col md:flex-row md:items-center justify-between gap-4 bg-white p-4 rounded-2xl shadow-sm border border-slate-200">
        <div>
          <h1 className="text-2xl font-black text-slate-900">Tu Ubicación</h1>
          <p className="text-slate-500 text-sm">Zonas de riesgo en tiempo real</p>
        </div>
        <div className="relative w-full md:w-96">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-slate-400" />
          <input 
            type="text" 
            placeholder="Buscar lugares o direcciones..." 
            className="w-full bg-slate-100 border-none rounded-xl pl-10 pr-4 py-3 text-sm focus:ring-2 focus:ring-indigo-500 font-medium text-slate-900"
          />
        </div>
      </header>

      {/* Map Simulation Area */}
      <div className="flex-1 bg-white rounded-3xl border border-slate-200 overflow-hidden relative shadow-sm min-h-[450px]">
        {/* Background Image Simulating a Map */}
        <div className="absolute inset-0 w-full h-full bg-slate-100 opacity-60">
           <img 
            src="https://images.unsplash.com/photo-1759802524049-2421ddaee0fe?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxjaXR5JTIwbWFwJTIwbmF2aWdhdGlvbnxlbnwxfHx8fDE3NzQ2MTMxMTB8MA&ixlib=rb-4.1.0&q=80&w=1080" 
            className="w-full h-full object-cover grayscale opacity-50 contrast-125" 
            alt="City Map" 
          />
        </div>

        {/* Floating Pins (Heatmap/Risk Zones) */}
        <div className="absolute top-[20%] left-[30%] -translate-x-1/2 -translate-y-1/2 flex flex-col items-center">
          <div key="pulse1" className="bg-red-500/20 p-6 rounded-full absolute -z-10 animate-pulse"></div>
          <div key="icon1" className="bg-red-600 text-white p-2 rounded-full shadow-lg mb-1">
            <AlertTriangle className="w-5 h-5" />
          </div>
          <div key="label1" className="bg-white px-3 py-1 rounded-lg text-xs font-bold shadow-md border border-slate-100">Robos recientes</div>
        </div>

        {/* User Location & Real-time Tracking */}
        <div className="absolute top-[50%] left-[50%] -translate-x-1/2 -translate-y-1/2 flex flex-col items-center">
          <div key="pulse2" className="w-16 h-16 bg-indigo-500/20 rounded-full flex items-center justify-center animate-pulse absolute -z-10"></div>
          <div key="icon2" className="bg-indigo-600 text-white p-3 rounded-full shadow-lg border-2 border-white">
            <UserCircle className="w-6 h-6" />
          </div>
          <div key="label2" className="mt-2 bg-indigo-900 text-white px-3 py-1 rounded-full text-xs font-bold shadow-lg">Tú</div>
        </div>

        {/* Overlay Controls (Top Right) - Journey Sharing */}
        <div className="absolute top-6 right-6 flex flex-col gap-3">
          <button className="bg-white px-4 py-3 rounded-2xl shadow-lg border border-slate-100 text-indigo-700 hover:bg-slate-50 flex items-center gap-2 font-bold transition-all">
            <Navigation2 className="w-5 h-5" />
            <span className="hidden md:inline">Compartir Trayecto</span>
          </button>
        </div>

        {/* Overlay Controls (Bottom area) */}
        <div className="absolute bottom-6 left-6 right-6 flex items-end justify-between pointer-events-none">
          {/* Quick Route/Check-in */}
          <div className="flex flex-col gap-3 pointer-events-auto">
            <button className="bg-emerald-500 px-5 py-3.5 rounded-2xl shadow-lg text-white hover:bg-emerald-600 flex items-center gap-2 font-black transition-all hover:-translate-y-1">
              <CheckCircle2 className="w-6 h-6" />
              Llegué Bien
            </button>
          </div>

          {/* SOS & Recenter */}
          <div className="flex flex-col items-end gap-4 pointer-events-auto">
            <button className="bg-white p-3.5 rounded-xl shadow-lg border border-slate-100 text-slate-700 hover:bg-slate-50">
              <MapPin className="w-6 h-6" />
            </button>
            <button className="bg-red-600 p-4 rounded-2xl shadow-lg text-white hover:bg-red-700 flex items-center justify-center animate-bounce hover:animate-none transition-all border-4 border-red-200 hover:scale-105">
              <ShieldAlert className="w-8 h-8" />
              <span className="ml-2 font-black text-lg">SOS</span>
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}
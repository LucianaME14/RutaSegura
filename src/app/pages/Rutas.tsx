import { useState, useEffect } from "react";
import { Navigation, MapPin, Search, ShieldCheck, Clock, Activity, Footprints, Bike, AlertTriangle, Navigation2, CheckCircle2, ShieldAlert, Phone, Map } from "lucide-react";

export function Rutas() {
  // Estados para simular el flujo completo
  const [step, setStep] = useState<"buscar" | "alternativas" | "navegando">("buscar");
  const [isSearching, setIsSearching] = useState(false);
  const [mode, setMode] = useState<"pedestrian" | "bike">("pedestrian");

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault();
    setIsSearching(true);
    setTimeout(() => {
      setIsSearching(false);
      setStep("alternativas");
    }, 1500);
  };

  const startNavigation = () => {
    setStep("navegando");
  };

  // Pantalla 17: Seguimiento en tiempo real + Pantalla 18: SOS + Pantalla 19: Check-in
  if (step === "navegando") {
    return (
      <div className="h-[calc(100vh-120px)] flex flex-col animate-in fade-in zoom-in-95 duration-500 relative">
        {/* Cabecera de Navegación */}
        <div className="bg-indigo-600 text-white p-4 rounded-t-3xl shadow-lg z-10 flex items-center gap-4">
          <div className="bg-indigo-800 p-3 rounded-xl">
            <Navigation className="w-8 h-8 text-indigo-200" />
          </div>
          <div className="flex-1">
            <h2 className="text-2xl font-black">Sigue derecho</h2>
            <p className="text-indigo-200 font-medium text-sm">hacia Av. Universitaria • Llegada: 18:45 (15 min)</p>
          </div>
          <div className="bg-emerald-500 px-3 py-1 rounded-lg border border-emerald-400">
            <span className="text-xs font-bold block text-center">Score</span>
            <span className="text-lg font-black">98%</span>
          </div>
        </div>

        {/* Mapa Simulado de Navegación */}
        <div className="flex-1 bg-slate-100 relative overflow-hidden border-x border-slate-200">
          <img 
            src="https://images.unsplash.com/photo-1759802524049-2421ddaee0fe?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxjaXR5JTIwbWFwJTIwbmF2aWdhdGlvbnxlbnwxfHx8fDE3NzQ2MTMxMTB8MA&ixlib=rb-4.1.0&q=80&w=1080" 
            className="w-full h-full object-cover opacity-80" 
            alt="City Map Navigation" 
          />
          
          {/* Ruta dibujada simulada */}
          <div className="absolute top-1/4 left-1/4 w-1/2 h-1/2 border-l-8 border-b-8 border-indigo-600 rounded-bl-3xl opacity-80 z-0"></div>

          {/* Marcador del usuario */}
          <div className="absolute top-[40%] left-[25%] -translate-x-1/2 -translate-y-1/2 flex flex-col items-center">
            <div className="w-20 h-20 bg-indigo-500/30 rounded-full animate-ping absolute -z-10"></div>
            <div className="bg-white text-indigo-600 p-2 rounded-full shadow-xl border-4 border-indigo-600 rotate-45">
              <Navigation className="w-6 h-6 fill-indigo-600" />
            </div>
          </div>
          
          {/* Alerta en ruta */}
          <div className="absolute top-[25%] left-[50%] flex items-center gap-2 bg-amber-100 border border-amber-300 text-amber-800 px-3 py-2 rounded-xl shadow-lg font-bold text-xs">
            <AlertTriangle className="w-4 h-4" /> Desvío por obra
          </div>

          {/* Botón flotante Compartir (Pantalla 16) */}
          <button className="absolute top-4 right-4 bg-white/90 backdrop-blur px-4 py-3 rounded-2xl shadow-lg border border-slate-200 text-indigo-700 hover:bg-white flex items-center gap-2 font-bold transition-all">
            <Navigation2 className="w-5 h-5" />
            <span className="hidden md:inline">Compartir Trayecto</span>
          </button>
        </div>

        {/* Panel Inferior: SOS y Llegué Bien */}
        <div className="bg-white p-6 rounded-b-3xl shadow-[0_-10px_40px_rgba(0,0,0,0.1)] border-t border-slate-200 z-10 flex gap-4">
          <button 
            className="flex-1 bg-red-600 hover:bg-red-700 text-white font-black py-4 rounded-2xl flex items-center justify-center gap-2 text-lg transition-all"
            onClick={() => alert("¡ALERTA SOS ENVIADA A CONTACTOS Y POLICÍA!")}
          >
            <ShieldAlert className="w-6 h-6" /> SOS
          </button>
          <button 
            className="flex-1 bg-emerald-500 hover:bg-emerald-600 text-white font-black py-4 rounded-2xl flex items-center justify-center gap-2 text-lg transition-all"
            onClick={() => {
              alert("Notificación 'Llegué bien' enviada a tus contactos.");
              setStep("buscar");
            }}
          >
            <CheckCircle2 className="w-6 h-6" /> Llegué Bien
          </button>
          <button 
            onClick={() => setStep("alternativas")}
            className="bg-slate-100 text-slate-600 p-4 rounded-2xl hover:bg-slate-200"
          >
            <Map className="w-6 h-6" />
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className="space-y-6 animate-in fade-in slide-in-from-bottom-4 duration-500 h-full flex flex-col">
      <header>
        <h1 className="text-3xl font-black text-slate-900">
          {step === "buscar" ? "Buscar Ruta Segura" : "Rutas Alternativas"}
        </h1>
        <p className="text-slate-500 mt-2 font-medium">
          {step === "buscar" 
            ? "Encuentra la mejor forma de ir de un punto A a un punto B evitando zonas de riesgo."
            : "Selecciona la opción que mejor se adapte a tu necesidad."}
        </p>
      </header>

      {/* Search Panel (Pantalla 6) */}
      <div className="bg-white p-6 rounded-3xl border border-slate-200 shadow-sm relative z-10">
        <form onSubmit={handleSearch} className="space-y-5">
          <div className="relative pl-8 space-y-4">
            <div className="absolute left-3.5 top-5 bottom-5 w-0.5 bg-slate-200 rounded-full"></div>
            <div className="relative">
              <div className="absolute -left-[27px] top-1/2 -translate-y-1/2 w-4 h-4 rounded-full border-4 border-indigo-600 bg-white"></div>
              <input type="text" defaultValue="Mi ubicación actual" className="w-full bg-slate-50 border border-slate-200 rounded-2xl px-4 py-3.5 text-slate-900 font-medium focus:outline-none focus:ring-2 focus:ring-indigo-500" />
            </div>
            <div className="relative">
              <div className="absolute -left-[27px] top-1/2 -translate-y-1/2 w-4 h-4 rounded-full bg-red-500 border-2 border-white shadow-sm"></div>
              <input type="text" placeholder="¿A dónde vas? (Ej: Universidad)" required className="w-full bg-slate-50 border border-slate-200 rounded-2xl px-4 py-3.5 text-slate-900 font-medium focus:outline-none focus:ring-2 focus:ring-indigo-500" />
            </div>
          </div>

          {step === "buscar" && (
            <div className="flex items-center gap-4 border-t border-slate-100 pt-5">
              <div className="flex bg-slate-100 p-1 rounded-xl flex-1 md:flex-none">
                <button type="button" onClick={() => setMode("pedestrian")} className={`flex-1 md:flex-none flex items-center justify-center gap-2 px-6 py-2.5 rounded-lg font-bold transition-all ${mode === 'pedestrian' ? 'bg-white shadow-sm text-indigo-700' : 'text-slate-500'}`}>
                  <Footprints className="w-5 h-5" /> Peatón
                </button>
                <button type="button" onClick={() => setMode("bike")} className={`flex-1 md:flex-none flex items-center justify-center gap-2 px-6 py-2.5 rounded-lg font-bold transition-all ${mode === 'bike' ? 'bg-white shadow-sm text-indigo-700' : 'text-slate-500'}`}>
                  <Bike className="w-5 h-5" /> Bicicleta
                </button>
              </div>
              <button type="submit" disabled={isSearching} className="ml-auto bg-indigo-600 text-white font-bold px-8 py-3.5 rounded-xl hover:bg-indigo-700 flex items-center gap-2 shadow-md">
                {isSearching ? <Activity className="w-5 h-5 animate-spin" /> : <Search className="w-5 h-5" />}
                Buscar
              </button>
            </div>
          )}
        </form>
      </div>

      {/* Results Section (Pantallas 7 y 8) */}
      {step === "alternativas" && (
        <div className="space-y-4 animate-in fade-in slide-in-from-bottom-8 duration-700">
          <div className="flex justify-between items-center">
            <h2 className="text-xl font-black text-slate-900 flex items-center gap-2">
              Resultados <span className="text-sm font-bold bg-indigo-100 text-indigo-700 px-2 py-0.5 rounded-full">3 Opciones</span>
            </h2>
            <button onClick={() => setStep("buscar")} className="text-sm font-bold text-slate-500 hover:text-indigo-600">Nueva búsqueda</button>
          </div>
          
          <div className="grid md:grid-cols-3 gap-5">
            {/* Opción 1: Más Segura */}
            <div className="bg-white rounded-3xl border-2 border-emerald-500 shadow-[0_8px_30px_rgba(16,185,129,0.15)] p-1 overflow-hidden relative group">
              <div className="absolute top-0 right-0 bg-emerald-500 text-white text-xs font-black px-3 py-1 rounded-bl-xl z-10">MÁS SEGURA</div>
              <div className="p-5">
                <div className="flex items-center gap-3 mb-4">
                  <div className="p-2.5 bg-emerald-100 rounded-xl text-emerald-600">
                    <ShieldCheck className="w-6 h-6" />
                  </div>
                  <div>
                    <h3 className="font-black text-slate-900 text-lg">Ruta Principal</h3>
                    <p className="text-emerald-600 text-xs font-bold">Nivel de Seguridad: Alto</p>
                  </div>
                </div>
                <p className="text-slate-500 text-sm font-medium mb-6">Evita 2 zonas oscuras y usa vías principales iluminadas.</p>
                <div className="flex justify-between items-center bg-slate-50 p-3 rounded-xl mb-4">
                  <span className="font-bold text-sm flex items-center gap-1"><Clock className="w-4 h-4" /> 22 min</span>
                  <span className="font-bold text-sm flex items-center gap-1"><Activity className="w-4 h-4" /> 1.8 km</span>
                </div>
                <button onClick={startNavigation} className="w-full bg-emerald-500 text-white font-bold py-3.5 rounded-xl flex justify-center gap-2">
                  Iniciar Navegación <Navigation className="w-4 h-4" />
                </button>
              </div>
            </div>

            {/* Opción 2: Más Rápida */}
            <div className="bg-white rounded-3xl border border-slate-200 shadow-sm p-1">
              <div className="absolute top-0 right-0 bg-amber-500 text-white text-xs font-black px-3 py-1 rounded-bl-xl z-10">MÁS RÁPIDA</div>
              <div className="p-5">
                <div className="flex items-center gap-3 mb-4">
                  <div className="p-2.5 bg-amber-100 rounded-xl text-amber-600"><Clock className="w-6 h-6" /></div>
                  <div>
                    <h3 className="font-black text-slate-900 text-lg">Atajo Sur</h3>
                    <p className="text-amber-600 text-xs font-bold">Nivel de Seguridad: Medio</p>
                  </div>
                </div>
                <p className="text-slate-500 text-sm font-medium mb-6">El trayecto más corto, cruza cerca a 1 zona con reportes.</p>
                <div className="flex justify-between items-center bg-slate-50 p-3 rounded-xl mb-4">
                  <span className="font-bold text-sm flex items-center gap-1"><Clock className="w-4 h-4" /> 14 min</span>
                  <span className="font-bold text-sm flex items-center gap-1"><Activity className="w-4 h-4" /> 1.2 km</span>
                </div>
                <button onClick={startNavigation} className="w-full bg-slate-900 text-white font-bold py-3.5 rounded-xl flex justify-center gap-2">
                  Ver Ruta
                </button>
              </div>
            </div>

            {/* Opción 3: Equilibrada */}
            <div className="bg-white rounded-3xl border border-slate-200 shadow-sm p-1">
              <div className="absolute top-0 right-0 bg-blue-500 text-white text-xs font-black px-3 py-1 rounded-bl-xl z-10">EQUILIBRADA</div>
              <div className="p-5">
                <div className="flex items-center gap-3 mb-4">
                  <div className="p-2.5 bg-blue-100 rounded-xl text-blue-600"><Navigation className="w-6 h-6" /></div>
                  <div>
                    <h3 className="font-black text-slate-900 text-lg">Ruta Escénica</h3>
                    <p className="text-blue-600 text-xs font-bold">Nivel de Seguridad: Medio-Alto</p>
                  </div>
                </div>
                <p className="text-slate-500 text-sm font-medium mb-6">Buen balance. Usa calles secundarias con cámaras.</p>
                <div className="flex justify-between items-center bg-slate-50 p-3 rounded-xl mb-4">
                  <span className="font-bold text-sm flex items-center gap-1"><Clock className="w-4 h-4" /> 18 min</span>
                  <span className="font-bold text-sm flex items-center gap-1"><Activity className="w-4 h-4" /> 1.5 km</span>
                </div>
                <button onClick={startNavigation} className="w-full bg-slate-900 text-white font-bold py-3.5 rounded-xl flex justify-center gap-2">
                  Ver Ruta
                </button>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
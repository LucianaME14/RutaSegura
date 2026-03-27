import { useState } from "react";
import { Camera, MapPin, AlertTriangle, Send, CheckCircle2, Navigation, ArrowLeft } from "lucide-react";
import { useNavigate } from "react-router";

export function Reportar() {
  const [step, setStep] = useState<"formulario" | "confirmacion">("formulario");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const navigate = useNavigate();

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    setIsSubmitting(true);
    // Simular el tiempo de envío al backend
    setTimeout(() => {
      setIsSubmitting(false);
      setStep("confirmacion");
    }, 1500);
  };

  if (step === "confirmacion") {
    return (
      <div className="flex flex-col items-center justify-center text-center py-20 animate-in fade-in zoom-in duration-500 max-w-md mx-auto">
        <div className="w-24 h-24 bg-emerald-100 rounded-full flex items-center justify-center mb-6 shadow-sm border-4 border-emerald-50">
          <CheckCircle2 className="w-12 h-12 text-emerald-500" />
        </div>
        <h2 className="text-3xl font-black text-slate-900 mb-3">¡Reporte Enviado!</h2>
        <p className="text-slate-500 font-medium mb-8 leading-relaxed">
          Tu reporte ha sido registrado exitosamente. Nuestra IA lo está analizando para alertar a otros usuarios de Ruta Segura en la zona.
        </p>
        
        <div className="flex flex-col w-full gap-3">
          <button 
            onClick={() => navigate("/perfil")}
            className="w-full bg-indigo-600 hover:bg-indigo-700 text-white font-bold py-4 rounded-xl transition-all shadow-sm"
          >
            Ver mis reportes
          </button>
          <button 
            onClick={() => setStep("formulario")}
            className="w-full bg-slate-100 hover:bg-slate-200 text-slate-700 font-bold py-4 rounded-xl transition-all"
          >
            Hacer otro reporte
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className="space-y-6 animate-in fade-in slide-in-from-bottom-4 duration-500 max-w-2xl mx-auto pb-10">
      <header>
        <h1 className="text-3xl font-black text-slate-900">Crear Reporte</h1>
        <p className="text-slate-500 mt-2 font-medium">Ayuda a la comunidad reportando incidencias en tu ruta actual.</p>
      </header>

      <form onSubmit={handleSubmit} className="bg-white p-6 md:p-8 rounded-3xl border border-slate-200 shadow-sm space-y-6">
        
        {/* Tipo de Incidencia */}
        <div className="space-y-3">
          <label className="text-sm font-bold text-slate-900 flex items-center gap-2">
            <AlertTriangle className="w-4 h-4 text-amber-500" /> 
            ¿Qué sucedió?
          </label>
          <div className="grid grid-cols-2 md:grid-cols-3 gap-3">
            {[
              { id: "robo", label: "Robo", bg: "bg-red-50 text-red-700 border-red-200" },
              { id: "acoso", label: "Acoso", bg: "bg-purple-50 text-purple-700 border-purple-200" },
              { id: "luz", label: "Sin Iluminación", bg: "bg-slate-800 text-slate-200 border-slate-700" },
              { id: "hueco", label: "Hueco en Vía", bg: "bg-amber-50 text-amber-700 border-amber-200" },
              { id: "accidente", label: "Accidente", bg: "bg-orange-50 text-orange-700 border-orange-200" },
              { id: "otro", label: "Otro peligro", bg: "bg-slate-100 text-slate-700 border-slate-200" }
            ].map((tipo) => (
              <label key={tipo.id} className={`cursor-pointer border-2 rounded-xl p-3 text-center transition-all hover:scale-105 active:scale-95 flex flex-col items-center justify-center ${tipo.bg} has-[:checked]:ring-2 has-[:checked]:ring-offset-2 has-[:checked]:ring-indigo-500`}>
                <input type="radio" name="tipo_incidencia" value={tipo.id} className="sr-only" required />
                <span className="font-bold text-sm">{tipo.label}</span>
              </label>
            ))}
          </div>
        </div>

        {/* Ubicación (Simulada autodetectada) */}
        <div className="space-y-3">
          <label className="text-sm font-bold text-slate-900 flex items-center gap-2">
            <MapPin className="w-4 h-4 text-indigo-500" /> 
            Ubicación
          </label>
          <div className="flex bg-slate-50 border border-slate-200 rounded-xl p-1">
            <input 
              type="text" 
              defaultValue="Av. Universitaria, cuadra 12" 
              className="w-full bg-transparent border-none px-3 py-2 text-slate-700 font-medium focus:outline-none"
            />
            <button type="button" className="bg-white px-4 py-2 rounded-lg text-sm font-bold text-indigo-600 border border-slate-200 shadow-sm flex items-center gap-1">
              <Navigation className="w-4 h-4" /> Usar GPS
            </button>
          </div>
        </div>

        {/* Evidencia (Foto) */}
        <div className="space-y-3">
          <label className="text-sm font-bold text-slate-900 flex items-center gap-2">
            <Camera className="w-4 h-4 text-emerald-500" /> 
            Evidencia (Opcional)
          </label>
          <div className="border-2 border-dashed border-slate-200 rounded-2xl p-8 flex flex-col items-center justify-center text-slate-500 hover:bg-slate-50 transition-colors cursor-pointer group">
            <div className="bg-white p-3 rounded-full shadow-sm mb-3 group-hover:scale-110 transition-transform">
              <Camera className="w-6 h-6 text-slate-400" />
            </div>
            <p className="font-bold text-sm">Toma una foto o sube un archivo</p>
            <p className="text-xs mt-1">Soporta JPG, PNG</p>
          </div>
        </div>

        {/* Detalles / Descripción */}
        <div className="space-y-3">
          <label className="text-sm font-bold text-slate-900">
            Detalles adicionales
          </label>
          <textarea 
            rows={3} 
            placeholder="Describe brevemente lo que viste para ayudar a otros usuarios..."
            className="w-full bg-slate-50 border border-slate-200 rounded-xl px-4 py-3 text-slate-700 font-medium focus:outline-none focus:ring-2 focus:ring-indigo-500 resize-none"
          ></textarea>
        </div>

        {/* Reporte Anónimo */}
        <label className="flex items-center gap-3 p-4 bg-slate-50 rounded-xl border border-slate-100 cursor-pointer">
          <input type="checkbox" className="w-5 h-5 text-indigo-600 rounded border-slate-300 focus:ring-indigo-500" />
          <div>
            <p className="font-bold text-slate-900 text-sm">Enviar como Anónimo</p>
            <p className="text-xs text-slate-500 font-medium mt-0.5">Tu nombre no será visible para otros usuarios.</p>
          </div>
        </label>

        {/* Botón Enviar */}
        <button 
          type="submit" 
          disabled={isSubmitting}
          className="w-full bg-indigo-600 hover:bg-indigo-700 text-white font-black py-4 rounded-xl transition-all flex items-center justify-center gap-2 shadow-md disabled:opacity-70"
        >
          {isSubmitting ? (
            <span className="animate-pulse">Enviando reporte...</span>
          ) : (
            <>Enviar Reporte <Send className="w-5 h-5" /></>
          )}
        </button>
      </form>
    </div>
  );
}
import { useState, useRef, useEffect } from "react";
import { MessageSquare, X, Send, ShieldAlert, Sparkles } from "lucide-react";
import { motion, AnimatePresence } from "motion/react";

export function SafeBot() {
  const [isOpen, setIsOpen] = useState(false);
  const [messages, setMessages] = useState([
    {
      text: "Hola. Soy la IA de Ruta Segura. Puedo ayudarte a encontrar la ruta más segura, informarte de zonas de riesgo o guiarte para hacer un reporte. ¿En qué te ayudo?",
      sender: "bot",
    },
  ]);
  const [input, setInput] = useState("");
  const messagesEndRef = useRef<HTMLDivElement>(null);

  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: "smooth" });
  };

  useEffect(() => {
    scrollToBottom();
  }, [messages]);

  const handleSend = (e: React.FormEvent) => {
    e.preventDefault();
    if (!input.trim()) return;

    const newMsgs = [...messages, { text: input, sender: "user" }];
    setMessages(newMsgs);
    setInput("");

    // Simulate AI / ML Recommendation response
    setTimeout(() => {
      let reply =
        "Entendido. Recuerda que siempre puedes usar el Botón SOS si te sientes en peligro inminente.";
      const lower = input.toLowerCase();

      if (
        lower.includes("universidad") ||
        lower.includes("ruta") ||
        lower.includes("ir a")
      ) {
        reply =
          "He analizado los datos históricos y reportes recientes. La ruta más segura hacia la Universidad en este horario (noche) es caminando por la Avenida Principal (Score: 95%). Evita el Parque Sur porque han reportado falta de iluminación hace 30 minutos.";
      } else if (
        lower.includes("peligro") ||
        lower.includes("robaron") ||
        lower.includes("ayuda")
      ) {
        reply =
          "⚠️ Por favor, busca un lugar seguro de inmediato. Si estás en emergencia presiona el botón SOS en el mapa para alertar a tus Contactos de Emergencia (Mamá, Hermano) y a la policía.";
      } else if (
        lower.includes("reportar") ||
        lower.includes("hueco") ||
        lower.includes("basura")
      ) {
        reply =
          "Claro, para reportar ve a la pestaña 'Reportar'. Clasificaremos tu reporte usando nuestra IA para notificar rápidamente a otros usuarios en la zona.";
      } else if (
        lower.includes("contacto") ||
        lower.includes("llegue") ||
        lower.includes("bien")
      ) {
        reply =
          "Al terminar tu recorrido, presiona 'Llegué Bien' en la pantalla del Mapa y notificaremos automáticamente a tus contactos de confianza.";
      }

      setMessages((prev) => [...prev, { text: reply, sender: "bot" }]);
    }, 1200);
  };

  return (
    <div className="fixed bottom-24 md:bottom-6 right-4 md:right-6 z-50">
      <AnimatePresence>
        {isOpen && (
          <motion.div
            initial={{ opacity: 0, y: 20, scale: 0.95 }}
            animate={{ opacity: 1, y: 0, scale: 1 }}
            exit={{ opacity: 0, y: 20, scale: 0.95 }}
            className="absolute bottom-20 right-0 w-[350px] bg-white rounded-3xl shadow-[0_10px_40px_rgba(0,0,0,0.15)] border border-slate-200 overflow-hidden flex flex-col h-[480px]"
          >
            <div className="bg-gradient-to-r from-indigo-700 to-indigo-900 p-4 text-white flex items-center justify-between shadow-sm z-10 relative">
              <div className="flex items-center gap-3">
                <div className="bg-white/20 p-2 rounded-xl backdrop-blur-sm">
                  <Sparkles className="w-5 h-5 text-white" />
                </div>
                <div>
                  <span className="font-black block leading-tight">
                    SafeBot IA
                  </span>
                  <span className="text-xs text-indigo-200 font-medium">
                    Asistente predictivo de rutas
                  </span>
                </div>
              </div>

              <button
                type="button"
                onClick={() => setIsOpen(false)}
                aria-label="Cerrar chat"
                title="Cerrar chat"
                className="hover:bg-white/10 p-2 rounded-xl transition-colors text-slate-200 hover:text-white"
              >
                <X className="w-5 h-5" />
              </button>
            </div>

            <div className="flex-1 p-5 overflow-y-auto space-y-5 bg-slate-50 text-sm flex flex-col">
              {messages.map((msg, idx) => (
                <div
                  key={idx}
                  className={`flex ${
                    msg.sender === "user" ? "justify-end" : "justify-start"
                  }`}
                >
                  <div
                    className={`max-w-[85%] rounded-2xl px-4 py-3 shadow-sm font-medium leading-relaxed ${
                      msg.sender === "user"
                        ? "bg-indigo-600 text-white rounded-tr-sm"
                        : "bg-white border border-slate-200 text-slate-800 rounded-tl-sm"
                    }`}
                  >
                    {msg.text}
                  </div>
                </div>
              ))}
              <div ref={messagesEndRef} />
            </div>

            <form
              onSubmit={handleSend}
              className="p-3 bg-white border-t border-slate-200 flex items-center gap-2"
            >
              <div className="flex-1 relative">
                <input
                  type="text"
                  value={input}
                  onChange={(e) => setInput(e.target.value)}
                  placeholder="Pregunta por una ruta segura..."
                  className="w-full bg-slate-100 border-none rounded-2xl px-4 py-3.5 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500 font-medium text-slate-900"
                />
              </div>

              <button
                type="submit"
                disabled={!input.trim()}
                aria-label="Enviar mensaje"
                title="Enviar mensaje"
                className="bg-indigo-600 text-white p-3.5 rounded-2xl hover:bg-indigo-700 transition-colors flex-shrink-0 shadow-sm disabled:opacity-50 disabled:cursor-not-allowed"
              >
                <Send className="w-5 h-5" />
              </button>
            </form>
          </motion.div>
        )}
      </AnimatePresence>

      <motion.button
        type="button"
        whileHover={{ scale: 1.05 }}
        whileTap={{ scale: 0.95 }}
        onClick={() => setIsOpen(!isOpen)}
        aria-label={isOpen ? "Cerrar SafeBot" : "Abrir SafeBot"}
        title={isOpen ? "Cerrar SafeBot" : "Abrir SafeBot"}
        className="w-16 h-16 bg-indigo-600 text-white rounded-2xl shadow-xl flex items-center justify-center hover:bg-indigo-700 transition-colors border border-indigo-500 group relative"
      >
        {isOpen ? <X className="w-7 h-7" /> : <Sparkles className="w-7 h-7" />}

        {!isOpen && (
          <span className="absolute -top-2 -right-2 flex h-4 w-4">
            <span className="animate-ping absolute inline-flex h-full w-full rounded-full bg-red-400 opacity-75"></span>
            <span className="relative inline-flex rounded-full h-4 w-4 bg-red-500 border-2 border-white"></span>
          </span>
        )}
      </motion.button>
    </div>
  );
}

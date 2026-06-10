import { useState, useRef, useEffect } from "react";
import { X, Send, Sparkles, Loader2, Bot } from "lucide-react";
import { motion, AnimatePresence } from "motion/react";
import { useAuth } from "../contexts/AuthContext";
import { apiUrl, authJsonHeaders, readApiErrorMessage } from "../lib/api";

type ChatMessage = {
  text: string;
  sender: "user" | "bot";
  meta?: string;
};

const WELCOME_USER =
  "Hola. Soy SafeBot, tu asistente con IA de Ruta Segura. Puedo ayudarte con zonas seguras, rutas recomendadas, reportes cercanos, clima y el botón SOS. ¿En qué te ayudo?";

const WELCOME_ADMIN =
  "Hola. Soy SafeBot para administración. Puedo resumir reportes, usuarios, alertas activas y zonas con más incidentes. ¿Qué necesitas consultar?";

export function SafeBot() {
  const { token, isAdmin } = useAuth();
  const [isOpen, setIsOpen] = useState(false);
  const [messages, setMessages] = useState<ChatMessage[]>([]);
  const [input, setInput] = useState("");
  const [loading, setLoading] = useState(false);
  const [llmStatus, setLlmStatus] = useState<string | null>(null);
  const messagesEndRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (isOpen && messages.length === 0) {
      setMessages([
        {
          text: isAdmin ? WELCOME_ADMIN : WELCOME_USER,
          sender: "bot",
        },
      ]);
    }
  }, [isOpen, isAdmin, messages.length]);

  useEffect(() => {
    if (!isOpen || !token) return;
    fetch(apiUrl("/api/chat/status"), {
      headers: authJsonHeaders(token),
    })
      .then((r) => (r.ok ? r.json() : null))
      .then((data) => {
        if (data?.disponible) {
          setLlmStatus(`IA local: ${data.modelo}`);
        } else {
          setLlmStatus("Modo datos (sin Ollama)");
        }
      })
      .catch(() => setLlmStatus(null));
  }, [isOpen, token]);

  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: "smooth" });
  };

  useEffect(() => {
    scrollToBottom();
  }, [messages, loading]);

  const handleSend = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!input.trim() || loading || !token) return;

    const userText = input.trim();
    setMessages((prev) => [...prev, { text: userText, sender: "user" }]);
    setInput("");
    setLoading(true);

    try {
      const res = await fetch(apiUrl("/api/chat"), {
        method: "POST",
        headers: authJsonHeaders(token),
        body: JSON.stringify({ message: userText }),
      });

      if (!res.ok) {
        const err = await readApiErrorMessage(res, "No se pudo contactar a SafeBot.");
        setMessages((prev) => [
          ...prev,
          { text: err, sender: "bot" },
        ]);
        return;
      }

      const data = (await res.json()) as {
        answer?: string;
        llmActivo?: boolean;
        modelo?: string;
        desdeCache?: boolean;
      };

      const meta =
        data.llmActivo && data.modelo
          ? `${data.modelo}${data.desdeCache ? " · caché" : ""}`
          : undefined;

      setMessages((prev) => [
        ...prev,
        {
          text: data.answer ?? "Sin respuesta del servidor.",
          sender: "bot",
          meta,
        },
      ]);
    } catch {
      setMessages((prev) => [
        ...prev,
        {
          text: "Error de conexión. Verifica que el backend esté activo y que Ollama esté corriendo si usas IA local.",
          sender: "bot",
        },
      ]);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="fixed bottom-24 md:bottom-6 right-4 md:right-6 z-50">
      <AnimatePresence>
        {isOpen && (
          <motion.div
            initial={{ opacity: 0, y: 20, scale: 0.95 }}
            animate={{ opacity: 1, y: 0, scale: 1 }}
            exit={{ opacity: 0, y: 20, scale: 0.95 }}
            className="absolute bottom-20 right-0 w-[min(380px,calc(100vw-2rem))] bg-white rounded-3xl shadow-[0_10px_40px_rgba(0,0,0,0.15)] border border-slate-200 overflow-hidden flex flex-col h-[min(520px,70vh)]"
          >
            <div className="bg-gradient-to-r from-indigo-700 to-indigo-900 p-4 text-white flex items-center justify-between shadow-sm z-10 relative">
              <div className="flex items-center gap-3">
                <div className="bg-white/20 p-2 rounded-xl backdrop-blur-sm">
                  <Sparkles className="w-5 h-5 text-white" />
                </div>
                <div>
                  <span className="font-black block leading-tight">SafeBot IA</span>
                  <span className="text-xs text-indigo-200 font-medium">
                    {isAdmin
                      ? "Semantic Kernel · Admin"
                      : "Semantic Kernel · Ollama"}
                  </span>
                  {llmStatus && (
                    <span className="text-[10px] text-indigo-300 block mt-0.5">
                      {llmStatus}
                    </span>
                  )}
                </div>
              </div>
              <button
                type="button"
                onClick={() => setIsOpen(false)}
                className="hover:bg-white/10 p-2 rounded-xl transition-colors text-slate-200 hover:text-white"
                aria-label="Cerrar chat"
              >
                <X className="w-5 h-5" />
              </button>
            </div>

            <div className="flex-1 p-5 overflow-y-auto space-y-4 bg-slate-50 text-sm flex flex-col">
              {messages.map((msg, idx) => (
                <div
                  key={idx}
                  className={`flex ${msg.sender === "user" ? "justify-end" : "justify-start"}`}
                >
                  <div
                    className={`max-w-[88%] rounded-2xl px-4 py-3 shadow-sm font-medium leading-relaxed ${
                      msg.sender === "user"
                        ? "bg-indigo-600 text-white rounded-tr-sm"
                        : "bg-white border border-slate-200 text-slate-800 rounded-tl-sm"
                    }`}
                  >
                    {msg.sender === "bot" && (
                      <Bot className="w-3.5 h-3.5 text-indigo-500 mb-1 inline-block mr-1" />
                    )}
                    <span className="whitespace-pre-wrap">{msg.text}</span>
                    {msg.meta && (
                      <p className="text-[10px] text-slate-400 mt-2 font-normal">
                        {msg.meta}
                      </p>
                    )}
                  </div>
                </div>
              ))}
              {loading && (
                <div className="flex justify-start">
                  <div className="bg-white border border-slate-200 rounded-2xl px-4 py-3 flex items-center gap-2 text-slate-500">
                    <Loader2 className="w-4 h-4 animate-spin text-indigo-500" />
                    <span className="text-xs font-medium">SafeBot analizando…</span>
                  </div>
                </div>
              )}
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
                  placeholder={
                    isAdmin
                      ? "Ej: ¿Cuántos reportes hay hoy?"
                      : "Ej: ¿Es segura esta zona?"
                  }
                  disabled={loading}
                  className="w-full bg-slate-100 border-none rounded-2xl px-4 py-3.5 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500 font-medium text-slate-900 disabled:opacity-60"
                />
              </div>
              <button
                type="submit"
                disabled={!input.trim() || loading}
                className="bg-indigo-600 text-white p-3.5 rounded-2xl hover:bg-indigo-700 transition-colors flex-shrink-0 shadow-sm disabled:opacity-50 disabled:cursor-not-allowed"
                aria-label="Enviar mensaje"
              >
                {loading ? (
                  <Loader2 className="w-5 h-5 animate-spin" />
                ) : (
                  <Send className="w-5 h-5" />
                )}
              </button>
            </form>
          </motion.div>
        )}
      </AnimatePresence>

      <motion.button
        whileHover={{ scale: 1.05 }}
        whileTap={{ scale: 0.95 }}
        onClick={() => setIsOpen(!isOpen)}
        className="w-16 h-16 bg-indigo-600 text-white rounded-2xl shadow-xl flex items-center justify-center hover:bg-indigo-700 transition-colors border border-indigo-500 group relative"
        aria-label={isOpen ? "Cerrar SafeBot" : "Abrir SafeBot"}
      >
        {isOpen ? <X className="w-7 h-7" /> : <Sparkles className="w-7 h-7" />}
        {!isOpen && (
          <span className="absolute -top-2 -right-2 flex h-4 w-4">
            <span className="animate-ping absolute inline-flex h-full w-full rounded-full bg-emerald-400 opacity-75" />
            <span className="relative inline-flex rounded-full h-4 w-4 bg-emerald-500 border-2 border-white" />
          </span>
        )}
      </motion.button>
    </div>
  );
}

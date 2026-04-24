import { ArrowLeft, ScrollText, CheckCircle, Scale, Ban } from "lucide-react";
import { Link, useNavigate } from "react-router";

export default function Terminos() {
  const navigate = useNavigate();

  return (
    <div className="min-h-screen bg-slate-900 text-slate-300 font-sans selection:bg-indigo-500/30 selection:text-indigo-200">
      <div className="fixed inset-0 pointer-events-none overflow-hidden">
        <div className="absolute top-[10%] left-[80%] w-[30%] h-[30%] rounded-full bg-purple-600/20 blur-[120px]" />
        <div className="absolute bottom-[-10%] right-[-10%] w-[50%] h-[50%] rounded-full bg-indigo-600/10 blur-[120px]" />
      </div>

      <div className="relative z-10 max-w-4xl mx-auto px-6 py-12 sm:py-20">
        <button
          onClick={() => navigate(-1)}
          className="flex items-center gap-2 text-indigo-400 hover:text-indigo-300 transition-colors mb-8 group"
        >
          <div className="p-2 rounded-full bg-indigo-500/10 group-hover:bg-indigo-500/20 transition-colors">
            <ArrowLeft className="w-5 h-5" />
          </div>
          <span className="font-medium">Volver</span>
        </button>

        <div className="bg-slate-800/50 backdrop-blur-xl border border-slate-700/50 rounded-3xl p-8 sm:p-12 shadow-2xl">
          <div className="flex items-center gap-4 mb-8">
            <div className="p-4 rounded-2xl bg-indigo-500/20 text-indigo-400 border border-indigo-500/30">
              <ScrollText className="w-8 h-8" />
            </div>
            <div>
              <h1 className="text-3xl sm:text-4xl font-bold text-white tracking-tight">
                Términos de Servicio
              </h1>
              <p className="text-slate-400 mt-2">
                Versión 1.2 — Efectiva a partir de Marzo 2026
              </p>
            </div>
          </div>

          <div className="space-y-8 text-slate-300 leading-relaxed">
            <section>
              <h2 className="text-xl font-semibold text-white mb-4 flex items-center gap-2">
                <CheckCircle className="w-5 h-5 text-indigo-400" />
                1. Aceptación de los Términos
              </h2>
              <p>
                Al acceder o utilizar <strong>Ruta Segura</strong>, plataforma
                para la seguridad estudiantil, usted acepta estar obligado por
                estos Términos de Servicio y a todas las leyes y regulaciones
                aplicables. Si no está de acuerdo con alguno de estos términos,
                tiene prohibido utilizar la plataforma.
              </p>
            </section>

            <section>
              <h2 className="text-xl font-semibold text-white mb-4 flex items-center gap-2">
                <Scale className="w-5 h-5 text-indigo-400" />
                2. Licencia de Uso
              </h2>
              <p className="mb-4">
                Se concede permiso para descargar temporalmente una copia de la
                plataforma para su visualización personal y no comercial. Ésta
                es la concesión de una licencia, no una transferencia de título,
                y bajo esta licencia no se puede:
              </p>
              <ul className="list-disc pl-6 space-y-2">
                <li>
                  Modificar, copiar o reproducir los materiales ni el código
                  fuente.
                </li>
                <li>
                  Utilizar la plataforma para propósitos comerciales (excluyendo
                  a administradores autorizados).
                </li>
                <li>
                  Intentar descompilar, realizar ingeniería inversa u obtener
                  cualquier componente de Ruta Segura.
                </li>
                <li>Transferir la información personal de otros usuarios.</li>
              </ul>
            </section>

            <section>
              <h2 className="text-xl font-semibold text-white mb-4 flex items-center gap-2">
                <Ban className="w-5 h-5 text-indigo-400" />
                3. Uso Indebido y Penales
              </h2>
              <p>
                El Botón SOS y el sistema de reportes son herramientas críticas.
                La emisión de alertas falsas y reportes difamatorios maliciosos
                de manera intencionada resultará en la suspensión permanente de
                su cuenta y un reporte a las autoridades universitarias, quienes
                podrán emprender medidas disciplinarias adicionales.
              </p>
            </section>

            <section>
              <h2 className="text-xl font-semibold text-white mb-4">
                4. Limitación de Responsabilidad
              </h2>
              <p>
                Ruta Segura provee sugerencias de rutas, alertas tempranas e
                integraciones con IA para evaluar zonas de riesgo. Sin embargo,
                en ningún caso Ruta Segura o sus desarrolladores serán
                responsables de ningún daño ni garantizan total invulnerabilidad
                en entornos físicos. Usted utiliza la aplicación bajo su propio
                riesgo y discreción.
              </p>
            </section>

            <section>
              <h2 className="text-xl font-semibold text-white mb-4">
                5. Precisión de Materiales
              </h2>
              <p>
                El material visual (mapas de calor) e informativo, incluyendo
                sugerencias del asistente SafeBot, puede contener errores de
                sistema o técnicos. Ruta Segura no garantiza que todo material
                sea preciso o completo todo el tiempo. Nos reservamos el derecho
                de aplicar correcciones al sistema y a las API en cualquier
                momento, sin aviso previo.
              </p>
            </section>

            <section>
              <h2 className="text-xl font-semibold text-white mb-4">
                6. Modificaciones a los Términos
              </h2>
              <p>
                Ruta Segura puede revisar y actualizar estos Términos de
                Servicio para su plataforma en cualquier momento sin previo
                aviso. Al utilizar esta app, acepta quedar obligado a la versión
                actual vigente de estos Términos de Servicio.
              </p>
            </section>

            <div className="mt-12 pt-8 border-t border-slate-700/50 text-sm text-slate-500 text-center">
              Para consultas legales o denuncias:{" "}
              <a
                href="mailto:legal@rutasegura.edu"
                className="text-indigo-400 hover:text-indigo-300 transition-colors"
              >
                legal@rutasegura.edu
              </a>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

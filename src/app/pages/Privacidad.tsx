import { ArrowLeft, ShieldCheck, Lock, Eye, Database } from "lucide-react";
import { Link, useNavigate } from "react-router";

export default function Privacidad() {
  const navigate = useNavigate();

  return (
    <div className="min-h-screen bg-slate-900 text-slate-300 font-sans selection:bg-indigo-500/30 selection:text-indigo-200">
      <div className="fixed inset-0 pointer-events-none overflow-hidden">
        <div className="absolute top-[-10%] left-[-10%] w-[40%] h-[40%] rounded-full bg-indigo-600/20 blur-[120px]" />
        <div className="absolute bottom-[-10%] right-[-10%] w-[40%] h-[40%] rounded-full bg-blue-600/20 blur-[120px]" />
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
              <ShieldCheck className="w-8 h-8" />
            </div>
            <div>
              <h1 className="text-3xl sm:text-4xl font-bold text-white tracking-tight">
                Políticas de Privacidad
              </h1>
              <p className="text-slate-400 mt-2">
                Última actualización: Marzo 2026
              </p>
            </div>
          </div>

          <div className="space-y-8 text-slate-300 leading-relaxed">
            <section>
              <h2 className="text-xl font-semibold text-white mb-4 flex items-center gap-2">
                <Database className="w-5 h-5 text-indigo-400" />
                1. Recopilación de la Información
              </h2>
              <p>
                En <strong>Ruta Segura</strong>, recopilamos información
                personal que nos proporcionas directamente al registrarte, como
                tu nombre, correo electrónico universitario, y contraseña
                (encriptada). Además, al usar la aplicación, recopilamos datos
                de ubicación en tiempo real exclusivamente cuando activas el
                modo de ruta segura, reportas un incidente o presionas el botón
                SOS.
              </p>
            </section>

            <section>
              <h2 className="text-xl font-semibold text-white mb-4 flex items-center gap-2">
                <Eye className="w-5 h-5 text-indigo-400" />
                2. Uso de la Información
              </h2>
              <p className="mb-4">
                La información que recopilamos se utiliza con los siguientes
                propósitos:
              </p>
              <ul className="list-disc pl-6 space-y-2">
                <li>Brindar, operar y mantener la plataforma.</li>
                <li>
                  Mejorar la seguridad dentro de la comunidad universitaria.
                </li>
                <li>
                  Procesar y mostrar reportes de incidentes de manera anónima al
                  resto de usuarios.
                </li>
                <li>Enviar alertas tempranas en caso de riesgo en la zona.</li>
                <li>
                  Optimizar nuestros algoritmos de Inteligencia Artificial para
                  mapas de calor.
                </li>
              </ul>
            </section>

            <section>
              <h2 className="text-xl font-semibold text-white mb-4 flex items-center gap-2">
                <Lock className="w-5 h-5 text-indigo-400" />
                3. Protección y Seguridad
              </h2>
              <p>
                Implementamos medidas de seguridad técnicas y organizativas de
                nivel empresarial para proteger tus datos contra acceso no
                autorizado, alteración, divulgación o destrucción. Las
                contraseñas se almacenan mediante algoritmos de hash y las
                comunicaciones se realizan a través de canales cifrados (HTTPS).
                Los reportes de incidentes no revelan la identidad del usuario a
                la comunidad.
              </p>
            </section>

            <section>
              <h2 className="text-xl font-semibold text-white mb-4">
                4. Compartir Información con Terceros
              </h2>
              <p>
                No vendemos, intercambiamos ni transferimos a terceros tu
                información personal identificable. Esto no incluye a terceros
                de confianza que nos asisten en operar nuestro sitio web o
                llevar a cabo nuestro negocio, siempre que dichas partes
                acuerden mantener esta información confidencial. En casos de
                emergencia extrema (Botón SOS), la ubicación puede compartirse
                con autoridades de seguridad del campus.
              </p>
            </section>

            <section>
              <h2 className="text-xl font-semibold text-white mb-4">
                5. Derechos del Usuario
              </h2>
              <p>
                Tienes el derecho de acceder, corregir, actualizar o solicitar
                la eliminación de tu información personal en cualquier momento
                accediendo a la configuración de tu perfil o contactando a
                nuestro equipo de soporte. También puedes revocar en cualquier
                momento los permisos de geolocalización desde tu dispositivo
                móvil.
              </p>
            </section>

            <div className="mt-12 pt-8 border-t border-slate-700/50 text-sm text-slate-500 text-center">
              Si tienes preguntas sobre esta política, puedes contactarnos a{" "}
              <a
                href="mailto:soporte@rutasegura.edu"
                className="text-indigo-400 hover:text-indigo-300 transition-colors"
              >
                soporte@rutasegura.edu
              </a>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

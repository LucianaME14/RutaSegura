import {
  RadioReceiver,
  AlertTriangle,
  Construction,
  LightbulbOff,
  MessageSquare,
  ThumbsUp,
  MapPin,
  Clock,
} from "lucide-react";

const alerts = [
  {
    id: 1,
    type: "peligro",
    title: "Intento de robo a ciclista",
    location: "Parque de la Universidad (Sur)",
    time: "Hace 10 min",
    user: "Usuario Anónimo",
    likes: 24,
    comments: 5,
    icon: AlertTriangle,
    color: "bg-red-50 text-red-600 border-red-200",
    iconBg: "bg-red-100",
  },
  {
    id: 2,
    type: "obra",
    title: "Cráter gigante en la pista",
    location: "Av. Principal y Calle 8",
    time: "Hace 45 min",
    user: "Carlos M.",
    likes: 112,
    comments: 18,
    icon: Construction,
    image:
      "https://images.unsplash.com/photo-1696692118953-df89e9f639c9?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxwb3Rob2xlJTIwc3RyZWV0fGVufDF8fHx8MTc3NDYzNjIzOXww&ixlib=rb-4.1.0&q=80&w=1080",
    color: "bg-amber-50 text-amber-700 border-amber-200",
    iconBg: "bg-amber-100",
  },
  {
    id: 3,
    type: "luz",
    title: "Poste de luz dañado, cuadra oscura",
    location: "Pasaje Los Pinos",
    time: "Hace 2 horas",
    user: "Maria G.",
    likes: 45,
    comments: 2,
    icon: LightbulbOff,
    color: "bg-indigo-50 text-indigo-700 border-indigo-200",
    iconBg: "bg-indigo-100",
  },
];

export default function Alertas() {
  return (
    <div className="space-y-8 animate-in fade-in slide-in-from-bottom-4 duration-500">
      <header className="flex flex-col md:flex-row md:items-center justify-between gap-4">
        <div>
          <h1 className="text-3xl font-black text-slate-900 flex items-center gap-3">
            <RadioReceiver className="w-8 h-8 text-indigo-600" />
            Alertas Comunitarias
          </h1>
          <p className="text-slate-500 mt-2 font-medium">
            Información en tiempo real de tu zona y comunidad universitaria.
          </p>
        </div>
        <select className="bg-white border border-slate-200 text-sm text-slate-700 font-bold rounded-xl px-4 py-3 shadow-sm focus:ring-2 focus:ring-indigo-500">
          <option>Más recientes</option>
          <option>Cerca de mí</option>
          <option>Más votadas</option>
        </select>
      </header>

      <div className="grid md:grid-cols-3 gap-8">
        <div className="md:col-span-2 space-y-6">
          {alerts.map((alert) => (
            <div
              key={alert.id}
              className={`bg-white rounded-3xl border ${alert.color.replace("text-", "border-").replace("bg-", "border-").split(" ")[2]} shadow-sm overflow-hidden transition-all hover:shadow-md`}
            >
              <div className="p-6">
                <div className="flex items-start gap-4">
                  <div
                    className={`p-4 rounded-2xl ${alert.iconBg} flex-shrink-0`}
                  >
                    <alert.icon
                      className={`w-6 h-6 ${alert.color.split(" ")[1]}`}
                    />
                  </div>

                  <div className="flex-1">
                    <div className="flex items-center justify-between mb-1">
                      <span className="text-xs font-bold text-slate-400 uppercase tracking-wider">
                        {alert.user}
                      </span>
                      <span className="text-xs font-bold text-slate-400 flex items-center gap-1">
                        <Clock className="w-3 h-3" /> {alert.time}
                      </span>
                    </div>
                    <h3 className="text-xl font-bold text-slate-900 mb-2 leading-tight">
                      {alert.title}
                    </h3>
                    <p className="text-slate-500 text-sm font-medium flex items-center gap-1.5 mb-4">
                      <MapPin className="w-4 h-4 text-slate-400" />{" "}
                      {alert.location}
                    </p>

                    {alert.image && (
                      <div className="w-full h-48 rounded-2xl overflow-hidden mb-4 border border-slate-100">
                        <img
                          src={alert.image}
                          alt="Evidencia"
                          className="w-full h-full object-cover"
                        />
                      </div>
                    )}

                    <div className="flex items-center gap-6 mt-4 pt-4 border-t border-slate-100">
                      <button className="flex items-center gap-2 text-slate-500 hover:text-indigo-600 font-bold text-sm transition-colors">
                        <ThumbsUp className="w-5 h-5" /> {alert.likes}
                      </button>
                      <button className="flex items-center gap-2 text-slate-500 hover:text-indigo-600 font-bold text-sm transition-colors">
                        <MessageSquare className="w-5 h-5" /> {alert.comments}
                      </button>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          ))}
        </div>

        <div className="space-y-6">
          <div className="bg-slate-900 rounded-3xl p-6 text-white shadow-xl relative overflow-hidden">
            <div className="absolute top-0 right-0 p-8 opacity-10">
              <AlertTriangle className="w-32 h-32" />
            </div>
            <div className="relative z-10">
              <h3 className="text-xl font-black mb-2">Botón de Pánico</h3>
              <p className="text-slate-300 text-sm mb-6 font-medium leading-relaxed">
                Si te encuentras en una situación de emergencia real, presiona
                para alertar a la policía y a tus contactos.
              </p>
              <button className="w-full bg-red-600 text-white font-black py-4 rounded-2xl text-lg hover:bg-red-700 transition-all shadow-lg hover:shadow-red-600/50 hover:-translate-y-1 flex items-center justify-center gap-2">
                <AlertTriangle className="w-6 h-6" /> SOS Emergencia
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

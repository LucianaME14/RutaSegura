import { estilosRiesgo, confianzaTexto } from "../lib/riesgoMl";

type Props = {
  riesgo: string;
  confianza?: number | null;
  indicador?: string;
  compact?: boolean;
};

/** Badge visual para clasificación ML.NET (Segura / Moderada / Peligrosa). */
export function RiesgoMlBadge({ riesgo, confianza, indicador, compact }: Props) {
  const s = estilosRiesgo(riesgo);
  const icon = indicador?.trim() || s.icon;

  if (compact) {
    return (
      <span
        className={`inline-flex items-center gap-1 rounded-full border px-2 py-0.5 text-xs font-bold ${s.badge}`}
        title={confianza != null ? confianzaTexto(confianza) : undefined}
      >
        <span aria-hidden>{icon}</span>
        {s.label}
      </span>
    );
  }

  return (
    <span
      className={`inline-flex flex-col gap-0.5 rounded-xl border px-3 py-1.5 text-xs font-bold ${s.badge}`}
    >
      <span className="inline-flex items-center gap-1.5">
        <span className={`h-2 w-2 rounded-full ${s.dot}`} aria-hidden />
        <span>{icon}</span>
        {s.label}
      </span>
      {confianza != null ? (
        <span className="font-medium opacity-80">{confianzaTexto(confianza)}</span>
      ) : null}
    </span>
  );
}

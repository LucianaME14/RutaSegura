/** Etiquetas ML.NET de clasificación de zona (Model Builder — Data Classification). */

export type RiesgoNivel = "Segura" | "Moderada" | "Peligrosa";

export function estilosRiesgo(riesgo: string) {
  const r = riesgo.trim().toLowerCase();
  if (r.includes("segura") && !r.includes("moder")) {
    return {
      badge: "bg-emerald-100 text-emerald-800 border-emerald-200",
      dot: "bg-emerald-500",
      label: "Segura",
      icon: "🟢",
    };
  }
  if (r.includes("moder")) {
    return {
      badge: "bg-amber-100 text-amber-900 border-amber-200",
      dot: "bg-amber-500",
      label: "Moderada",
      icon: "🟡",
    };
  }
  return {
    badge: "bg-red-100 text-red-900 border-red-200",
    dot: "bg-red-500",
    label: "Peligrosa",
    icon: "🔴",
  };
}

export function confianzaTexto(confianza: number) {
  const pct = confianza <= 1 ? Math.round(confianza * 100) : Math.round(confianza);
  return `${pct}% confianza ML`;
}

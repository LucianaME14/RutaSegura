import type { LatLngLiteral } from "../types/maps";

export function hashString(s: string): number {
  let h = 0;
  for (let i = 0; i < s.length; i += 1) {
    h = (Math.imul(31, h) + s.charCodeAt(i)) | 0;
  }
  return Math.abs(h);
}

export function haversineKm(a: LatLngLiteral, b: LatLngLiteral): number {
  const R = 6371;
  const dLat = ((b.lat - a.lat) * Math.PI) / 180;
  const dLng = ((b.lng - a.lng) * Math.PI) / 180;
  const la1 = (a.lat * Math.PI) / 180;
  const la2 = (b.lat * Math.PI) / 180;
  const s =
    Math.sin(dLat / 2) ** 2 +
    Math.cos(la1) * Math.cos(la2) * Math.sin(dLng / 2) ** 2;
  return 2 * R * Math.asin(Math.sqrt(s));
}

/** Puntos para una ruta con curvatura (visual distinto por variante). */
export function buildRoutePath(
  a: LatLngLiteral,
  b: LatLngLiteral,
  variant: 0 | 1 | 2,
): LatLngLiteral[] {
  const pts: LatLngLiteral[] = [];
  const wobble =
    variant === 0 ? 0.0009 : variant === 1 ? 0.00045 : 0.0007;
  const phase = variant * 1.7;
  for (let i = 0; i <= 14; i += 1) {
    const t = i / 14;
    const lat = a.lat + (b.lat - a.lat) * t;
    const lng = a.lng + (b.lng - a.lng) * t;
    const perp = wobble * Math.sin(t * Math.PI * 2.5 + phase);
    const dx = b.lng - a.lng;
    const dy = b.lat - a.lat;
    const len = Math.hypot(dx, dy) || 1;
    const nx = -dy / len;
    const ny = dx / len;
    pts.push({
      lat: lat + nx * perp,
      lng: lng + ny * perp,
    });
  }
  return pts;
}

export const LIMA_CENTRO: LatLngLiteral = { lat: -12.046374, lng: -77.042793 };

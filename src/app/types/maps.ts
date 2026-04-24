export type LatLngLiteral = { lat: number; lng: number };

/** Caja de sesgo; compatible con `google.maps.LatLngBoundsLiteral`. */
export type LatLngBoundsLiteral = {
  south: number;
  west: number;
  north: number;
  east: number;
};

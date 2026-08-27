import * as L from 'leaflet';

// O Leaflet resolve os ícones padrão via caminhos relativos que quebram
// com bundlers modernos (Vite/webpack). Apontamos explicitamente para os
// assets publicados no CDN oficial do projeto.
// eslint-disable-next-line @typescript-eslint/no-explicit-any
delete (L.Icon.Default.prototype as any)._getIconUrl;

L.Icon.Default.mergeOptions({
  iconRetinaUrl: 'https://unpkg.com/leaflet@1.9.4/dist/images/marker-icon-2x.png',
  iconUrl: 'https://unpkg.com/leaflet@1.9.4/dist/images/marker-icon.png',
  shadowUrl: 'https://unpkg.com/leaflet@1.9.4/dist/images/marker-shadow.png'
});

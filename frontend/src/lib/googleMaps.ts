import { importLibrary, setOptions } from '@googlemaps/js-api-loader';

let promessaCarregamento: Promise<void> | null = null;

/**
 * Carrega o SDK do Google Maps uma única vez (as classes ficam disponíveis
 * em window.google.maps depois disso) — várias páginas podem chamar isso,
 * só a primeira chamada efetivamente injeta o script.
 *
 * Usa a API funcional (setOptions/importLibrary) porque a classe `Loader`
 * do @googlemaps/js-api-loader está deprecada na v2 do pacote.
 */
export function carregarGoogleMaps(): Promise<void> {
  if (!promessaCarregamento) {
    setOptions({ key: import.meta.env.VITE_GOOGLE_MAPS_API_KEY ?? '', v: 'weekly' });
    promessaCarregamento = Promise.all([
      importLibrary('maps'),
      importLibrary('marker'),
      importLibrary('routes')
    ]).then(() => undefined);
  }
  return promessaCarregamento;
}

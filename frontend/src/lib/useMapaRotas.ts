import { useEffect, useRef, type RefObject } from 'react';
import { carregarGoogleMaps } from './googleMaps';

export interface ParadaParaDesenhar {
  latitude: number;
  longitude: number;
  nome: string;
}

export interface RotaParaDesenhar {
  paradas: ParadaParaDesenhar[];
  cor: string;
  linhaCodigo?: string;
}

/**
 * Desenha uma ou mais rotas de ônibus num Google Map: o traçado real das
 * ruas entre as paradas + um marcador por parada com InfoWindow. Compartilhado
 * por OnibusDisponiveis, MapaRotas e ReportarLotacao.
 *
 * Usa a Routes API nova (`google.maps.routes.Route.computeRoutes`), não o
 * `DirectionsService`/`DirectionsRenderer` legado — esse foi depreciado pelo
 * Google e, em projetos novos, costuma vir desabilitado por padrão (retorna
 * REQUEST_DENIED mesmo com a Maps JavaScript API habilitada).
 */
export function useMapaRotas(
  containerRef: RefObject<HTMLDivElement | null>,
  centroPadrao: [number, number],
  zoomPadrao = 13
) {
  const mapaRef = useRef<google.maps.Map | null>(null);
  const prontoRef = useRef<Promise<google.maps.Map> | null>(null);
  const polylinesRef = useRef<google.maps.Polyline[]>([]);
  const marcadoresRef = useRef<google.maps.Marker[]>([]);

  function obterMapa(): Promise<google.maps.Map> {
    if (!prontoRef.current) {
      prontoRef.current = carregarGoogleMaps().then(() => {
        if (!containerRef.current) {
          throw new Error('Contêiner do mapa não está montado.');
        }
        const mapa = new google.maps.Map(containerRef.current, {
          center: { lat: centroPadrao[0], lng: centroPadrao[1] },
          zoom: zoomPadrao
        });
        mapaRef.current = mapa;
        return mapa;
      });
    }
    return prontoRef.current;
  }

  function limparDesenho() {
    polylinesRef.current.forEach((p) => p.setMap(null));
    marcadoresRef.current.forEach((m) => m.setMap(null));
    polylinesRef.current = [];
    marcadoresRef.current = [];
  }

  useEffect(() => {
    obterMapa();
    return () => {
      limparDesenho();
      mapaRef.current = null;
      prontoRef.current = null;
    };
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  async function desenharRotas(rotas: RotaParaDesenhar[]) {
    const mapa = await obterMapa();
    limparDesenho();

    const bounds = new google.maps.LatLngBounds();
    let totalPontos = 0;

    for (const rota of rotas) {
      rota.paradas.forEach((parada) => {
        const posicao = { lat: parada.latitude, lng: parada.longitude };
        bounds.extend(posicao);
        totalPontos += 1;

        const marcador = new google.maps.Marker({ position: posicao, map: mapa, title: parada.nome });
        const infoWindow = new google.maps.InfoWindow({
          content: rota.linhaCodigo ? `${rota.linhaCodigo} · ${parada.nome}` : parada.nome
        });
        marcador.addListener('click', () => infoWindow.open(mapa, marcador));
        marcadoresRef.current.push(marcador);
      });

      if (rota.paradas.length > 1) {
        const origem = rota.paradas[0];
        const destino = rota.paradas[rota.paradas.length - 1];
        const intermediates = rota.paradas.slice(1, -1).map((p) => ({
          location: { lat: p.latitude, lng: p.longitude }
        }));

        try {
          const { routes } = await google.maps.routes.Route.computeRoutes({
            origin: { lat: origem.latitude, lng: origem.longitude },
            destination: { lat: destino.latitude, lng: destino.longitude },
            intermediates,
            travelMode: google.maps.TravelMode.DRIVING,
            fields: ['path']
          });

          const caminho = routes?.[0]?.path;
          if (caminho && caminho.length > 0) {
            const polyline = new google.maps.Polyline({
              map: mapa,
              path: caminho,
              strokeColor: rota.cor,
              strokeWeight: 4
            });
            polylinesRef.current.push(polyline);
          }
        } catch {
          // Se a Routes API falhar (chave sem a API habilitada, sem rota
          // viável entre os pontos etc.), os marcadores das paradas
          // continuam visíveis mesmo sem o traçado da rota.
        }
      }
    }

    if (totalPontos > 1) {
      mapa.fitBounds(bounds, 24);
    }
  }

  return { desenharRotas };
}

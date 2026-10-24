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
 * ruas entre as paradas (via Directions API, não linha reta) + um marcador
 * por parada com InfoWindow. Compartilhado por OnibusDisponiveis, MapaRotas
 * e ReportarLotacao — as três páginas que antes duplicavam essa lógica com
 * Leaflet.
 */
export function useMapaRotas(
  containerRef: RefObject<HTMLDivElement | null>,
  centroPadrao: [number, number],
  zoomPadrao = 13
) {
  const mapaRef = useRef<google.maps.Map | null>(null);
  const prontoRef = useRef<Promise<google.maps.Map> | null>(null);
  const renderersRef = useRef<google.maps.DirectionsRenderer[]>([]);
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
    renderersRef.current.forEach((r) => r.setMap(null));
    marcadoresRef.current.forEach((m) => m.setMap(null));
    renderersRef.current = [];
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
        const waypoints = rota.paradas.slice(1, -1).map((p) => ({
          location: { lat: p.latitude, lng: p.longitude },
          stopover: true
        }));

        const renderer = new google.maps.DirectionsRenderer({
          map: mapa,
          suppressMarkers: true,
          polylineOptions: { strokeColor: rota.cor, strokeWeight: 4 }
        });
        renderersRef.current.push(renderer);

        try {
          const directionsService = new google.maps.DirectionsService();
          const resultado = await directionsService.route({
            origin: { lat: origem.latitude, lng: origem.longitude },
            destination: { lat: destino.latitude, lng: destino.longitude },
            waypoints,
            travelMode: google.maps.TravelMode.DRIVING
          });
          renderer.setDirections(resultado);
        } catch {
          // Se a Directions API falhar (chave sem a API habilitada, sem
          // rota viável entre os pontos etc.), os marcadores das paradas
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

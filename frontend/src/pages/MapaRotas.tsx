import { useEffect, useRef, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import * as L from 'leaflet';
import { api } from '../lib/api';
import { Header } from '../components/Header';
import type { Rota } from '../types';
import './MapaRotas.css';

const CENTRO_PADRAO: [number, number] = [-23.6486, -46.5389];

export function MapaRotas() {
  const { linhaId } = useParams<{ linhaId: string }>();
  const mapaRef = useRef<HTMLDivElement>(null);
  const [rota, setRota] = useState<Rota | null>(null);
  const navigate = useNavigate();

  useEffect(() => {
    if (!linhaId || !mapaRef.current) return;

    const mapa = L.map(mapaRef.current).setView(CENTRO_PADRAO, 14);
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      attribution: '&copy; OpenStreetMap contributors',
      maxZoom: 19
    }).addTo(mapa);

    api.get<Rota>(`/rotas/${linhaId}`).then(({ data }) => {
      setRota(data);
      const paradasOrdenadas = [...data.paradas].sort((a, b) => a.ordem - b.ordem);
      const pontos = paradasOrdenadas.map((p) => [p.latitude, p.longitude] as [number, number]);

      paradasOrdenadas.forEach((parada) => {
        L.marker([parada.latitude, parada.longitude]).addTo(mapa).bindPopup(parada.nome);
      });

      if (pontos.length > 1) {
        L.polyline(pontos, { color: '#1e4d94', weight: 4 }).addTo(mapa);
        mapa.fitBounds(L.latLngBounds(pontos), { padding: [24, 24] });
      }
    });

    return () => {
      mapa.remove();
    };
  }, [linhaId]);

  return (
    <>
      <Header titulo={rota ? `${rota.linhaCodigo} · ${rota.linhaNome}` : 'Mapa de rotas'} />

      <main className="mapa-rotas">
        <div ref={mapaRef} className="mapa-rotas__mapa" />

        <div className="mapa-rotas__acoes">
          <button className="btn" onClick={() => navigate(`/reportar?linhaId=${linhaId}`)}>
            Reportar lotação desta linha
          </button>
        </div>
      </main>
    </>
  );
}

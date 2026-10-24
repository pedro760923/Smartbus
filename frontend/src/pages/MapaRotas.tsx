import { useEffect, useRef, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { api } from '../lib/api';
import { useMapaRotas } from '../lib/useMapaRotas';
import { Header } from '../components/Header';
import type { Rota } from '../types';
import './MapaRotas.css';

const CENTRO_PADRAO: [number, number] = [-23.6486, -46.5389];
const COR_ROTA = '#1e4d94';

export function MapaRotas() {
  const { linhaId } = useParams<{ linhaId: string }>();
  const mapaRef = useRef<HTMLDivElement>(null);
  const { desenharRotas } = useMapaRotas(mapaRef, CENTRO_PADRAO, 14);
  const [rota, setRota] = useState<Rota | null>(null);
  const navigate = useNavigate();

  useEffect(() => {
    if (!linhaId) return;

    api.get<Rota>(`/rotas/${linhaId}`).then(({ data }) => {
      setRota(data);
      const paradasOrdenadas = [...data.paradas].sort((a, b) => a.ordem - b.ordem);
      desenharRotas([{ paradas: paradasOrdenadas, cor: COR_ROTA, linhaCodigo: data.linhaCodigo }]);
    });
    // eslint-disable-next-line react-hooks/exhaustive-deps
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

import { useEffect, useRef, useState } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import * as L from 'leaflet';
import { api } from '../lib/api';
import { useAuth } from '../lib/auth';
import { NivelLotacao, type Linha, type NovoReporte, type Rota } from '../types';
import './ReportarLotacao.css';

const CENTRO_PADRAO: [number, number] = [-23.6547, -46.5382];

export function ReportarLotacao() {
  const mapaRef = useRef<HTMLDivElement>(null);
  const mapaInstancia = useRef<L.Map | null>(null);
  const [linhas, setLinhas] = useState<Linha[]>([]);
  const [searchParams] = useSearchParams();
  const [linhaSelecionadaId, setLinhaSelecionadaId] = useState<number | ''>('');
  const [nivelSelecionado, setNivelSelecionado] = useState<NivelLotacao | null>(null);
  const [erro, setErro] = useState('');
  const [enviando, setEnviando] = useState(false);
  const { usuario } = useAuth();
  const navigate = useNavigate();

  useEffect(() => {
    if (!mapaRef.current) return;
    const mapa = L.map(mapaRef.current).setView(CENTRO_PADRAO, 14);
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      attribution: '&copy; OpenStreetMap contributors',
      maxZoom: 19
    }).addTo(mapa);
    mapaInstancia.current = mapa;

    api.get<Linha[]>('/linhas').then(({ data }) => {
      setLinhas(data);
      const linhaIdQuery = searchParams.get('linhaId');
      if (linhaIdQuery) {
        setLinhaSelecionadaId(Number(linhaIdQuery));
      }
    });

    return () => {
      mapa.remove();
      mapaInstancia.current = null;
    };
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  useEffect(() => {
    const mapa = mapaInstancia.current;
    if (!mapa || linhaSelecionadaId === '') return;

    api.get<Rota>(`/rotas/${linhaSelecionadaId}`).then(({ data: rota }) => {
      mapa.eachLayer((camada) => {
        if (!(camada instanceof L.TileLayer)) mapa.removeLayer(camada);
      });

      const paradasOrdenadas = [...rota.paradas].sort((a, b) => a.ordem - b.ordem);
      const pontos = paradasOrdenadas.map((p) => [p.latitude, p.longitude] as [number, number]);

      paradasOrdenadas.forEach((parada) => {
        L.marker([parada.latitude, parada.longitude]).addTo(mapa).bindPopup(parada.nome);
      });

      if (pontos.length > 1) {
        L.polyline(pontos, { color: '#1e4d94', weight: 4 }).addTo(mapa);
        mapa.fitBounds(L.latLngBounds(pontos), { padding: [24, 24] });
      }
    });
  }, [linhaSelecionadaId]);

  function obterLocalizacao(): Promise<{ latitude?: number; longitude?: number }> {
    return new Promise((resolve) => {
      if (!('geolocation' in navigator)) {
        resolve({});
        return;
      }
      navigator.geolocation.getCurrentPosition(
        (posicao) => resolve({ latitude: posicao.coords.latitude, longitude: posicao.coords.longitude }),
        () => resolve({})
      );
    });
  }

  async function enviar() {
    if (linhaSelecionadaId === '' || nivelSelecionado == null) return;
    setEnviando(true);
    setErro('');

    try {
      const { latitude, longitude } = await obterLocalizacao();
      const reporte: NovoReporte = {
        linhaId: linhaSelecionadaId,
        nivelLotacao: nivelSelecionado,
        latitude,
        longitude
      };
      await api.post('/reportes', reporte);
      navigate('/confirmacao');
    } catch {
      setErro('Não foi possível enviar seu reporte. Tente novamente.');
    } finally {
      setEnviando(false);
    }
  }

  return (
    <main className="reportar">
      <button className="voltar" onClick={() => navigate('/linhas')} aria-label="Voltar">
        ‹
      </button>

      <div ref={mapaRef} className="reportar__mapa" />

      <section className="painel">
        <h2>Como está sua viagem{usuario?.nome ? `, ${usuario.nome}` : ''}?</h2>

        <label className="reportar__campo">
          <select
            value={linhaSelecionadaId}
            onChange={(e) => setLinhaSelecionadaId(e.target.value ? Number(e.target.value) : '')}
          >
            <option value="" disabled>
              Selecione a linha
            </option>
            {linhas.map((linha) => (
              <option key={linha.id} value={linha.id}>
                {linha.codigo} · {linha.nome}
              </option>
            ))}
          </select>
        </label>

        <div className="opcoes-lotacao">
          <button
            type="button"
            className={`opcao opcao--verde ${nivelSelecionado === NivelLotacao.Vazio ? 'selecionada' : ''}`}
            onClick={() => setNivelSelecionado(NivelLotacao.Vazio)}
          >
            <span className="opcao__icone">🟢</span>
            Muitos assentos livres
          </button>
          <button
            type="button"
            className={`opcao opcao--amarelo ${nivelSelecionado === NivelLotacao.ComLugares ? 'selecionada' : ''}`}
            onClick={() => setNivelSelecionado(NivelLotacao.ComLugares)}
          >
            <span className="opcao__icone">🟡</span>
            Poucos assentos livres
          </button>
          <button
            type="button"
            className={`opcao opcao--vermelho ${nivelSelecionado === NivelLotacao.SuperLotado ? 'selecionada' : ''}`}
            onClick={() => setNivelSelecionado(NivelLotacao.SuperLotado)}
          >
            <span className="opcao__icone">🔴</span>
            Todos em pé / Lotado
          </button>
        </div>

        {erro && <p className="erro">{erro}</p>}

        <button
          className="btn btn--primario"
          disabled={linhaSelecionadaId === '' || nivelSelecionado == null || enviando}
          onClick={enviar}
        >
          {enviando ? 'Enviando...' : 'Enviar Reporte'}
        </button>
      </section>
    </main>
  );
}

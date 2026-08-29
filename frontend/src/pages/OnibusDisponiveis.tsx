import { useEffect, useRef, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import * as L from 'leaflet';
import { api } from '../lib/api';
import { NIVEL_LOTACAO_LABEL, NivelLotacao, type Linha, type Rota } from '../types';
import './OnibusDisponiveis.css';

const CENTRO_PADRAO: [number, number] = [-23.6547, -46.5382];
const CORES_ROTA = ['#1e4d94', '#2fa360', '#f2b705', '#8e5fd0', '#e0473e'];

type StatusLotacao = 'verde' | 'amarelo' | 'vermelho';

function statusLotacao(nivel?: NivelLotacao): StatusLotacao {
  if (nivel === NivelLotacao.SuperLotado) return 'vermelho';
  if (nivel === NivelLotacao.Cheio) return 'amarelo';
  return 'verde';
}

function labelLotacao(nivel?: NivelLotacao): string {
  if (nivel === NivelLotacao.SuperLotado) return 'Lotado';
  if (nivel === NivelLotacao.Cheio) return 'Médio';
  if (nivel === NivelLotacao.ComLugares) return 'Com lugares';
  return 'Vazio';
}

function IconePessoa({ status }: { status: StatusLotacao }) {
  const cor = status === 'vermelho' ? 'var(--sb-vermelho)' : status === 'amarelo' ? 'var(--sb-amarelo)' : 'var(--sb-verde)';
  return (
    <svg width="14" height="14" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
      <circle cx="12" cy="7" r="4" fill={cor} />
      <path d="M4 21c0-4.4 3.6-8 8-8s8 3.6 8 8" fill={cor} />
    </svg>
  );
}

export function OnibusDisponiveis() {
  const mapaRef = useRef<HTMLDivElement>(null);
  const mapaInstancia = useRef<L.Map | null>(null);
  const [linhas, setLinhas] = useState<Linha[]>([]);
  const [termo, setTermo] = useState('');
  const [coresPorLinha, setCoresPorLinha] = useState<Map<number, string>>(new Map());
  const navigate = useNavigate();

  useEffect(() => {
    if (!mapaRef.current || mapaInstancia.current) return;

    const mapa = L.map(mapaRef.current).setView(CENTRO_PADRAO, 13);
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      attribution: '&copy; OpenStreetMap contributors',
      maxZoom: 19
    }).addTo(mapa);
    mapaInstancia.current = mapa;

    return () => {
      mapa.remove();
      mapaInstancia.current = null;
    };
  }, []);

  useEffect(() => {
    buscar();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [termo]);

  function buscar() {
    api
      .get<Linha[]>('/linhas', { params: termo ? { termo } : undefined })
      .then((resp) => {
        setLinhas(resp.data);
        desenharRotas(resp.data);
      });
  }

  function desenharRotas(linhasParaDesenhar: Linha[]) {
    const mapa = mapaInstancia.current;
    if (!mapa || linhasParaDesenhar.length === 0) return;

    Promise.all(linhasParaDesenhar.map((linha) => api.get<Rota>(`/rotas/${linha.id}`))).then((respostas) => {
      const novasCores = new Map<number, string>();
      const todosPontos: [number, number][] = [];

      mapa.eachLayer((camada) => {
        if (!(camada instanceof L.TileLayer)) mapa.removeLayer(camada);
      });

      respostas.forEach(({ data: rota }, indice) => {
        const cor = CORES_ROTA[indice % CORES_ROTA.length];
        novasCores.set(rota.linhaId, cor);

        const paradasOrdenadas = [...rota.paradas].sort((a, b) => a.ordem - b.ordem);
        const pontos = paradasOrdenadas.map((p) => [p.latitude, p.longitude] as [number, number]);
        pontos.forEach((ponto) => todosPontos.push(ponto));

        if (pontos.length > 1) {
          L.polyline(pontos, { color: cor, weight: 4 }).addTo(mapa);
        }

        paradasOrdenadas.forEach((parada) => {
          L.circleMarker([parada.latitude, parada.longitude], {
            radius: 5,
            color: cor,
            fillColor: cor,
            fillOpacity: 0.9
          })
            .addTo(mapa)
            .bindPopup(`${rota.linhaCodigo} · ${parada.nome}`);
        });
      });

      setCoresPorLinha(novasCores);
      if (todosPontos.length > 1) {
        mapa.fitBounds(L.latLngBounds(todosPontos), { padding: [24, 24] });
      }
    });
  }

  return (
    <main className="onibus-disponiveis">
      <button className="voltar" onClick={() => navigate('/principal')} aria-label="Voltar">
        ‹
      </button>

      <div ref={mapaRef} className="onibus-disponiveis__mapa" />

      <section className="painel">
        <div className="painel__cabecalho">
          <h3>Ônibus Disponíveis</h3>
          <input
            className="painel__busca"
            type="search"
            placeholder="Buscar por código ou nome"
            value={termo}
            onChange={(e) => setTermo(e.target.value)}
          />
        </div>

        <ul className="painel__lista">
          {linhas.map((linha) => {
            const status = statusLotacao(linha.nivelLotacaoAtual);
            return (
              <li key={linha.id} onClick={() => navigate(`/mapa-rotas/${linha.id}`)}>
                <span className="badge-linha" style={{ background: coresPorLinha.get(linha.id) ?? '#1e4d94' }}>
                  {linha.codigo}
                </span>
                <span className="painel__info">
                  <strong>{linha.nome}</strong>
                  <small>Ver rota e reportar lotação</small>
                </span>
                <span className="painel__status-coluna">
                  <span className={`status status--${status}`}>
                    <IconePessoa status={status} />
                    {labelLotacao(linha.nivelLotacaoAtual)}
                  </span>
                  {linha.totalVotos > 0 && (
                    <small className="painel__votos">
                      {linha.distribuicaoLotacao
                        .map((voto) => `${voto.quantidade} ${NIVEL_LOTACAO_LABEL[voto.nivel].toLowerCase()}`)
                        .join(' · ')}
                    </small>
                  )}
                </span>
              </li>
            );
          })}
          {linhas.length === 0 && <li className="painel__vazio">Nenhuma linha encontrada.</li>}
        </ul>
      </section>
    </main>
  );
}

import { useEffect, useRef, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { api } from '../lib/api';
import { useDebounce } from '../lib/useDebounce';
import { useMapaRotas } from '../lib/useMapaRotas';
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
  const { desenharRotas } = useMapaRotas(mapaRef, CENTRO_PADRAO, 13);
  const [linhas, setLinhas] = useState<Linha[]>([]);
  const [termo, setTermo] = useState('');
  const [coresPorLinha, setCoresPorLinha] = useState<Map<number, string>>(new Map());
  const navigate = useNavigate();
  const termoDebounced = useDebounce(termo, 400);

  useEffect(() => {
    buscar();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [termoDebounced]);

  function buscar() {
    api
      .get<Linha[]>('/linhas', { params: termoDebounced ? { termo: termoDebounced } : undefined })
      .then((resp) => {
        setLinhas(resp.data);
        desenharNoMapa(resp.data);
      });
  }

  function desenharNoMapa(linhasParaDesenhar: Linha[]) {
    if (linhasParaDesenhar.length === 0) return;

    Promise.all(linhasParaDesenhar.map((linha) => api.get<Rota>(`/rotas/${linha.id}`))).then((respostas) => {
      const novasCores = new Map<number, string>();

      const rotasParaDesenhar = respostas.map(({ data: rota }, indice) => {
        const cor = CORES_ROTA[indice % CORES_ROTA.length];
        novasCores.set(rota.linhaId, cor);

        const paradasOrdenadas = [...rota.paradas].sort((a, b) => a.ordem - b.ordem);
        return {
          paradas: paradasOrdenadas,
          cor,
          linhaCodigo: rota.linhaCodigo
        };
      });

      setCoresPorLinha(novasCores);
      desenharRotas(rotasParaDesenhar);
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

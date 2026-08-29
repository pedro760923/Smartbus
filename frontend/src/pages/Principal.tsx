import { useEffect, useRef, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { api } from '../lib/api';
import { useAuth } from '../lib/auth';
import { useDebounce } from '../lib/useDebounce';
import type { EnderecoSugestao, Parada } from '../types';
import './Principal.css';

export function Principal() {
  const [paradas, setParadas] = useState<Parada[]>([]);
  const [carregando, setCarregando] = useState(false);
  const [erroLocalizacao, setErroLocalizacao] = useState(false);
  const [termoBusca, setTermoBusca] = useState('');
  const [sugestoes, setSugestoes] = useState<EnderecoSugestao[]>([]);
  const [mostrarSugestoes, setMostrarSugestoes] = useState(false);
  const [enderecoSelecionado, setEnderecoSelecionado] = useState<string | null>(null);
  const [notaSelecionada, setNotaSelecionada] = useState(0);
  const [avaliacaoEnviada, setAvaliacaoEnviada] = useState(false);
  const { logout } = useAuth();
  const navigate = useNavigate();
  const termoDebounced = useDebounce(termoBusca, 400);
  const esconderSugestoesTimeout = useRef<ReturnType<typeof setTimeout> | undefined>(undefined);

  useEffect(() => {
    buscarPorLocalizacaoAtual();
  }, []);

  useEffect(() => {
    if (!mostrarSugestoes) return;

    api
      .get<EnderecoSugestao[]>('/enderecos', { params: termoDebounced ? { termo: termoDebounced } : undefined })
      .then((resp) => setSugestoes(resp.data))
      .catch(() => setSugestoes([]));
  }, [termoDebounced, mostrarSugestoes]);

  function buscarPorLocalizacaoAtual() {
    if (!('geolocation' in navigator)) {
      setErroLocalizacao(true);
      return;
    }

    setEnderecoSelecionado(null);
    setCarregando(true);
    navigator.geolocation.getCurrentPosition(
      (posicao) => {
        api
          .get<Parada[]>('/paradas/proximas', {
            params: { latitude: posicao.coords.latitude, longitude: posicao.coords.longitude }
          })
          .then((resp) => setParadas(resp.data))
          .finally(() => setCarregando(false));
      },
      () => {
        setErroLocalizacao(true);
        setCarregando(false);
      }
    );
  }

  function selecionarSugestao(sugestao: EnderecoSugestao) {
    setTermoBusca(sugestao.endereco);
    setMostrarSugestoes(false);
    setEnderecoSelecionado(sugestao.endereco);

    api.post('/enderecos/historico', {
      termo: termoBusca,
      endereco: sugestao.endereco,
      latitude: sugestao.latitude,
      longitude: sugestao.longitude
    });

    setCarregando(true);
    api
      .get<Parada[]>('/paradas/proximas', {
        params: { latitude: sugestao.latitude, longitude: sugestao.longitude }
      })
      .then((resp) => setParadas(resp.data))
      .finally(() => setCarregando(false));
  }

  function aoFocarBusca() {
    clearTimeout(esconderSugestoesTimeout.current);
    setMostrarSugestoes(true);
  }

  function aoDesfocarBusca() {
    // Atraso para o clique numa sugestão registrar antes do dropdown sumir.
    esconderSugestoesTimeout.current = setTimeout(() => setMostrarSugestoes(false), 150);
  }

  function irParaOnibusDisponiveis() {
    navigate('/linhas');
  }

  function sair() {
    logout();
    navigate('/boas-vindas');
  }

  return (
    <main className="home">
      <header className="home__topo">
        <svg width="30" height="30" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
          <rect x="3" y="4" width="18" height="12" rx="4" fill="#1e4d94" />
          <circle cx="7.5" cy="18.5" r="1.6" fill="#1e4d94" />
          <circle cx="16.5" cy="18.5" r="1.6" fill="#1e4d94" />
          <path d="M6 8h12M6 11.5h8" stroke="#fff" strokeWidth="1.4" strokeLinecap="round" />
        </svg>
        <div className="home__topo-marca">
          <strong>SmartBus</strong>
          <span>Santo André</span>
        </div>
        <button className="home__sair" onClick={sair} aria-label="Sair">
          Sair
        </button>
      </header>

      <div className="home__busca-container">
        <label className="home__busca">
          <span className="home__busca-icone">🔍</span>
          <input
            type="search"
            placeholder="Para onde vamos?"
            value={termoBusca}
            onChange={(e) => setTermoBusca(e.target.value)}
            onFocus={aoFocarBusca}
            onBlur={aoDesfocarBusca}
          />
        </label>

        {mostrarSugestoes && sugestoes.length > 0 && (
          <ul className="home__sugestoes">
            {sugestoes.map((sugestao) => (
              <li key={sugestao.endereco} onMouseDown={() => selecionarSugestao(sugestao)}>
                <span>{sugestao.doHistorico ? '🕑' : '📍'}</span>
                <span>{sugestao.endereco}</span>
              </li>
            ))}
          </ul>
        )}
      </div>

      <button className="cartao-destaque" onClick={irParaOnibusDisponiveis}>
        <span className="cartao-destaque__icone">🚌</span>
        <span className="cartao-destaque__texto">
          <strong>Ônibus disponíveis</strong>
          <small>Veja o mapa e a lotação em tempo real</small>
        </span>
        <span className="cartao-destaque__seta">›</span>
      </button>

      <section className="home__paradas">
        <div className="home__paradas-cabecalho">
          <h3>{enderecoSelecionado ? 'Paradas perto do endereço buscado' : 'Paradas Próximas'}</h3>
          {carregando && <small>localizando...</small>}
        </div>

        {enderecoSelecionado && (
          <p className="home__endereco-ativo">
            <span>📍 {enderecoSelecionado}</span>
            <button onClick={buscarPorLocalizacaoAtual}>Usar minha localização</button>
          </p>
        )}

        {erroLocalizacao && (
          <p className="aviso-caixa">
            Não foi possível acessar sua localização. Ative o GPS/permissão do navegador para ver as
            paradas mais próximas.
          </p>
        )}

        <ul className="lista-paradas">
          {paradas.map((parada) => (
            <li key={parada.id} onClick={irParaOnibusDisponiveis}>
              <span className="lista-paradas__icone">🚏</span>
              <span className="lista-paradas__info">
                <strong>{parada.nome}</strong>
                {parada.distanciaMetros != null && <small>{Math.round(parada.distanciaMetros)} m de você</small>}
              </span>
              <span className="lista-paradas__seta">›</span>
            </li>
          ))}
          {paradas.length === 0 && !carregando && !erroLocalizacao && (
            <li className="lista-paradas__vazio">Nenhuma parada encontrada nas proximidades.</li>
          )}
        </ul>
      </section>

      <section className="cartao-avaliacao">
        {avaliacaoEnviada ? (
          <p className="cartao-avaliacao__obrigado">Obrigado pela sua avaliação! ⭐</p>
        ) : (
          <>
            <p className="cartao-avaliacao__titulo">⭐ Avalie sua experiência</p>
            <div className="cartao-avaliacao__estrelas">
              {[1, 2, 3, 4, 5].map((estrela) => (
                <span
                  key={estrela}
                  className={estrela <= notaSelecionada ? 'ativa' : ''}
                  onClick={() => setNotaSelecionada(estrela)}
                >
                  ★
                </span>
              ))}
            </div>
            <button
              className="btn btn--primario"
              disabled={notaSelecionada === 0}
              onClick={() => setAvaliacaoEnviada(true)}
            >
              Enviar
            </button>
          </>
        )}
      </section>
    </main>
  );
}

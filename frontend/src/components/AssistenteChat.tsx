import { useEffect, useRef, useState, type FormEvent } from 'react';
import { api } from '../lib/api';
import type { MensagemChat, PerguntaChatRequest, RespostaChat } from '../types';
import './AssistenteChat.css';

const MENSAGEM_BOAS_VINDAS: MensagemChat = {
  papel: 'assistente',
  texto: 'Oi! Eu sou o assistente do SmartBus. Posso ajudar com horários, rotas e lotação das linhas. O que você quer saber?'
};

export function AssistenteChat() {
  const [aberto, setAberto] = useState(false);
  const [mensagens, setMensagens] = useState<MensagemChat[]>([MENSAGEM_BOAS_VINDAS]);
  const [entrada, setEntrada] = useState('');
  const [enviando, setEnviando] = useState(false);
  const listaRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    listaRef.current?.scrollTo({ top: listaRef.current.scrollHeight, behavior: 'smooth' });
  }, [mensagens, aberto]);

  async function enviar(e: FormEvent) {
    e.preventDefault();
    const texto = entrada.trim();
    if (!texto || enviando) return;

    const novasMensagens = [...mensagens, { papel: 'usuario', texto } as MensagemChat];
    setMensagens(novasMensagens);
    setEntrada('');
    setEnviando(true);

    try {
      const corpo: PerguntaChatRequest = {
        mensagem: texto,
        historico: mensagens.map((m) => ({ papel: m.papel, texto: m.texto }))
      };
      const { data } = await api.post<RespostaChat>('/assistente/mensagem', corpo);
      setMensagens((atual) => [...atual, { papel: 'assistente', texto: data.resposta }]);
    } catch {
      setMensagens((atual) => [
        ...atual,
        { papel: 'assistente', texto: 'Não consegui responder agora. Tenta de novo em alguns instantes.' }
      ]);
    } finally {
      setEnviando(false);
    }
  }

  return (
    <div className="assistente">
      {aberto && (
        <section className="assistente__painel">
          <header className="assistente__cabecalho">
            <span>🤖 Assistente SmartBus</span>
            <button type="button" onClick={() => setAberto(false)} aria-label="Fechar chat">
              ✕
            </button>
          </header>

          <div className="assistente__mensagens" ref={listaRef}>
            {mensagens.map((m, indice) => (
              <p key={indice} className={`assistente__bolha assistente__bolha--${m.papel}`}>
                {m.texto}
              </p>
            ))}
            {enviando && <p className="assistente__bolha assistente__bolha--assistente assistente__digitando">digitando...</p>}
          </div>

          <form className="assistente__entrada" onSubmit={enviar}>
            <input
              type="text"
              placeholder="Pergunte sobre horários, rotas, lotação..."
              value={entrada}
              onChange={(e) => setEntrada(e.target.value)}
              disabled={enviando}
            />
            <button type="submit" disabled={enviando || !entrada.trim()} aria-label="Enviar">
              ➤
            </button>
          </form>
        </section>
      )}

      <button
        type="button"
        className="assistente__bolinha"
        onClick={() => setAberto((valor) => !valor)}
        aria-label={aberto ? 'Fechar assistente' : 'Abrir assistente'}
      >
        {aberto ? '✕' : '💬'}
      </button>
    </div>
  );
}

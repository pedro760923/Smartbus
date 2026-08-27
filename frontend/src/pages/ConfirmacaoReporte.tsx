import { useNavigate } from 'react-router-dom';
import { useAuth } from '../lib/auth';
import './ConfirmacaoReporte.css';

export function ConfirmacaoReporte() {
  const { usuario } = useAuth();
  const navigate = useNavigate();

  function fechar() {
    navigate('/principal');
  }

  return (
    <main className="confirmacao">
      <button className="voltar" onClick={fechar} aria-label="Voltar">
        ‹
      </button>

      <div className="confirmacao__selo">
        <svg width="52" height="52" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
          <path d="M5 12.5 10 17 19 7" stroke="#fff" strokeWidth="2.6" strokeLinecap="round" strokeLinejoin="round" />
        </svg>
      </div>

      <h1>REPORTE CONFIRMADO!</h1>
      <h2>Obrigado, {usuario?.nome ?? 'você'}!</h2>
      <p>
        Seu reporte foi registrado com sucesso e já está ajudando nossa comunidade a melhorar as rotas
        para todos.
      </p>

      <div className="confirmacao__rodape">
        <span>🌿</span>
        Obrigado pela sua contribuição contínua!
      </div>

      <button className="btn btn--primario" onClick={fechar}>
        Fechar
      </button>
    </main>
  );
}

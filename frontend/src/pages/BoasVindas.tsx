import { Link } from 'react-router-dom';
import './BoasVindas.css';

export function BoasVindas() {
  return (
    <div className="aviso">
      <div className="aviso__ilustracao">
        <svg width="220" height="150" viewBox="0 0 220 150" fill="none" xmlns="http://www.w3.org/2000/svg">
          <ellipse cx="110" cy="138" rx="90" ry="8" fill="#eaf1fc" />
          {/* poste da parada */}
          <rect x="30" y="40" width="4" height="90" rx="2" fill="#c7d4ea" />
          <rect x="18" y="30" width="28" height="16" rx="4" fill="#1e4d94" />
          <rect x="22" y="34" width="20" height="3" rx="1.5" fill="#fff" />
          <rect x="22" y="39" width="14" height="3" rx="1.5" fill="#fff" />
          {/* pessoa */}
          <circle cx="95" cy="70" r="11" fill="#1e4d94" />
          <path d="M78 130v-30c0-10 8-18 17-18s17 8 17 18v30" fill="#2fa360" />
          {/* balão de fala */}
          <rect x="118" y="42" width="80" height="34" rx="12" fill="#fff" stroke="#e7ebf3" strokeWidth="2" />
          <path d="M128 76l-8 12 16-8z" fill="#fff" stroke="#e7ebf3" strokeWidth="2" />
          <circle cx="134" cy="59" r="6" fill="#34a853" />
          <path d="M131.5 59l1.8 2 3.2-4" stroke="#fff" strokeWidth="1.6" strokeLinecap="round" strokeLinejoin="round" />
          <text x="148" y="63" fontFamily="Poppins, sans-serif" fontSize="12" fontWeight="700" fill="#1b2333">
            ETA: 5 min
          </text>
        </svg>
      </div>

      <h1>
        Entenda o SmartBus
        <br />
        Santo André
      </h1>

      <ul className="aviso__passos">
        <li>
          <span className="aviso__icone">📍</span>
          Veja o ônibus no mapa
        </li>
        <li>
          <span className="aviso__icone">⏱️</span>
          Verifique o tempo e lotação
        </li>
        <li>
          <span className="aviso__icone">📣</span>
          Colabore e informe
        </li>
      </ul>

      <p className="aviso__legenda">Rastreamento em tempo real e lotação colaborativa na sua cidade.</p>

      <Link to="/login" className="btn btn--primario">
        COMEÇAR VIAGEM
      </Link>
      <p className="aviso__rodape">
        Já tem conta? <Link to="/login">Entrar</Link> · <Link to="/registro">Criar conta</Link>
      </p>
    </div>
  );
}

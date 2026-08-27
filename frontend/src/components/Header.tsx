import { useNavigate } from 'react-router-dom';
import { useAuth } from '../lib/auth';
import './Header.css';

export function Header({ titulo = 'SmartBus' }: { titulo?: string }) {
  const { autenticado, logout } = useAuth();
  const navigate = useNavigate();

  function sair() {
    logout();
    navigate('/boas-vindas');
  }

  return (
    <header className="app-header">
      <span className="app-header__titulo">{titulo}</span>
      {autenticado && (
        <button className="app-header__sair" onClick={sair} aria-label="Sair">
          Sair
        </button>
      )}
    </header>
  );
}

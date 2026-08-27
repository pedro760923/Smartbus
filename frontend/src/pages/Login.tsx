import { useState, type FormEvent } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../lib/auth';
import './Auth.css';

export function Login() {
  const [email, setEmail] = useState('');
  const [senha, setSenha] = useState('');
  const [erro, setErro] = useState('');
  const [carregando, setCarregando] = useState(false);
  const { login } = useAuth();
  const navigate = useNavigate();

  async function entrar(e: FormEvent) {
    e.preventDefault();
    setCarregando(true);
    setErro('');
    try {
      await login({ email, senha });
      navigate('/principal');
    } catch {
      setErro('E-mail ou senha inválidos.');
    } finally {
      setCarregando(false);
    }
  }

  return (
    <div className="auth-page">
      <h1>Entrar</h1>
      <form onSubmit={entrar}>
        <label>
          E-mail acadêmico
          <input
            type="email"
            required
            placeholder="voce@fsa.edu.br"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
          />
        </label>
        <label>
          Senha
          <input
            type="password"
            required
            minLength={6}
            placeholder="••••••••"
            value={senha}
            onChange={(e) => setSenha(e.target.value)}
          />
        </label>

        {erro && <p className="erro">{erro}</p>}

        <button className="btn" type="submit" disabled={carregando}>
          {carregando ? 'Entrando...' : 'Entrar'}
        </button>
      </form>
      <p className="auth-page__rodape">
        Ainda não tem conta? <Link to="/registro">Cadastre-se</Link>
      </p>
    </div>
  );
}

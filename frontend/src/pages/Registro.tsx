import { useState, type FormEvent } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../lib/auth';
import './Auth.css';

export function Registro() {
  const [nome, setNome] = useState('');
  const [email, setEmail] = useState('');
  const [senha, setSenha] = useState('');
  const [erro, setErro] = useState('');
  const [carregando, setCarregando] = useState(false);
  const { registrar } = useAuth();
  const navigate = useNavigate();

  async function criarConta(e: FormEvent) {
    e.preventDefault();
    setCarregando(true);
    setErro('');
    try {
      await registrar({ nome, email, senha });
      navigate('/principal');
    } catch {
      setErro('Não foi possível criar sua conta. Verifique os dados.');
    } finally {
      setCarregando(false);
    }
  }

  return (
    <div className="auth-page">
      <h1>Criar conta</h1>
      <form onSubmit={criarConta}>
        <label>
          Nome completo
          <input
            type="text"
            required
            placeholder="Seu nome"
            value={nome}
            onChange={(e) => setNome(e.target.value)}
          />
        </label>
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
            placeholder="mínimo 6 caracteres"
            value={senha}
            onChange={(e) => setSenha(e.target.value)}
          />
        </label>

        {erro && <p className="erro">{erro}</p>}

        <button className="btn" type="submit" disabled={carregando}>
          {carregando ? 'Criando...' : 'Criar conta'}
        </button>
      </form>
      <p className="auth-page__rodape">
        Já tem conta? <Link to="/login">Entrar</Link>
      </p>
    </div>
  );
}

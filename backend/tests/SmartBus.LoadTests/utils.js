// Funções compartilhadas entre os cenários de carga.
import http from 'k6/http';
import { check } from 'k6';

export const BASE_URL = __ENV.BASE_URL || 'https://localhost:7000';

/**
 * Registra um usuário único (evita colisão de e-mail entre VUs) e
 * devolve o token JWT já pronto para uso no header Authorization.
 */
export function autenticarNovoUsuario() {
  const email = `k6.${Date.now()}.${__VU}.${Math.random().toString(36).slice(2)}@fsa.edu.br`;

  const resposta = http.post(
    `${BASE_URL}/api/auth/registro`,
    JSON.stringify({ nome: 'Usuário K6', email, senha: 'senha12345' }),
    { headers: { 'Content-Type': 'application/json' } }
  );

  check(resposta, { 'registro retornou 200': (r) => r.status === 200 });

  const token = resposta.json('token');
  return { token, email };
}

export function headersAutenticados(token) {
  return {
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    },
  };
}

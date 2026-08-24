// Cenário: login — mede o custo de BCrypt.Verify sob carga (é
// intencionalmente lento por design de segurança; o objetivo aqui é
// achar o ponto em que essa lentidão vira gargalo real de throughput).
import http from 'k6/http';
import { check, sleep } from 'k6';
import { BASE_URL, autenticarNovoUsuario } from './utils.js';

export const options = {
  scenarios: {
    login_constante: {
      executor: 'constant-arrival-rate',
      rate: 20,
      timeUnit: '1s',
      duration: '30s',
      preAllocatedVUs: 20,
      maxVUs: 60,
    },
  },
  thresholds: {
    http_req_duration: ['p(95)<1000'],
  },
};

export function setup() {
  return autenticarNovoUsuario();
}

export default function (data) {
  const resposta = http.post(
    `${BASE_URL}/api/auth/login`,
    JSON.stringify({ email: data.email, senha: 'senha12345' }),
    { headers: { 'Content-Type': 'application/json' } }
  );

  check(resposta, { 'login 200': (r) => r.status === 200 });

  sleep(1);
}

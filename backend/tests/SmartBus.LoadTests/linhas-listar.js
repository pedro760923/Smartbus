// Cenário: leitura de listagem de linhas — o endpoint mais visitado do
// app (toda tela inicial passa por aqui). Foco em throughput de leitura.
import http from 'k6/http';
import { check, sleep } from 'k6';
import { BASE_URL, autenticarNovoUsuario, headersAutenticados } from './utils.js';

export const options = {
  scenarios: {
    leitura_constante: {
      executor: 'constant-vus',
      vus: 30,
      duration: '1m',
    },
  },
  thresholds: {
    http_req_duration: ['p(95)<400', 'p(99)<800'],
    http_req_failed: ['rate<0.01'],
  },
};

export function setup() {
  return autenticarNovoUsuario();
}

export default function (data) {
  const resposta = http.get(`${BASE_URL}/api/linhas`, headersAutenticados(data.token));

  check(resposta, {
    'status 200': (r) => r.status === 200,
    'retornou lista': (r) => Array.isArray(r.json()),
  });

  sleep(1);
}

// Cenário: escrita concorrente de reportes de lotação (crowdsourcing).
// É o caminho crítico de escrita do sistema — muitos alunos reportando
// ao mesmo tempo nos horários de pico. Testa se o ValidacaoService e o
// SaveChanges aguentam concorrência sem degradar latência.
import http from 'k6/http';
import { check, sleep } from 'k6';
import { BASE_URL, autenticarNovoUsuario, headersAutenticados } from './utils.js';

export const options = {
  scenarios: {
    pico_de_reportes: {
      executor: 'ramping-vus',
      startVUs: 0,
      stages: [
        { duration: '30s', target: 50 },  // rampa até simular horário de pico
        { duration: '1m', target: 50 },   // sustenta o pico
        { duration: '20s', target: 0 },   // desce
      ],
    },
  },
  thresholds: {
    http_req_duration: ['p(95)<600'],
    http_req_failed: ['rate<0.02'],
  },
};

const NIVEIS = [0, 1, 2, 3];

export function setup() {
  const auth = autenticarNovoUsuario();
  const linhasResp = http.get(`${BASE_URL}/api/linhas`, headersAutenticados(auth.token));
  const linhas = linhasResp.json();
  return { linhaId: linhas[0].id };
}

export default function (data) {
  const auth = autenticarNovoUsuario();
  const nivel = NIVEIS[Math.floor(Math.random() * NIVEIS.length)];

  const resposta = http.post(
    `${BASE_URL}/api/reportes`,
    JSON.stringify({ linhaId: data.linhaId, nivelLotacao: nivel }),
    headersAutenticados(auth.token)
  );

  check(resposta, { 'reporte aceito (200)': (r) => r.status === 200 });

  sleep(0.5);
}

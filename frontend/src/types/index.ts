export const NivelLotacao = {
  Vazio: 0,
  ComLugares: 1,
  Cheio: 2,
  SuperLotado: 3
} as const;

export type NivelLotacao = (typeof NivelLotacao)[keyof typeof NivelLotacao];

export const NIVEL_LOTACAO_LABEL: Record<NivelLotacao, string> = {
  [NivelLotacao.Vazio]: 'Vazio',
  [NivelLotacao.ComLugares]: 'Com lugares',
  [NivelLotacao.Cheio]: 'Cheio',
  [NivelLotacao.SuperLotado]: 'Superlotado'
};

export interface Usuario {
  id: number;
  nome: string;
  email: string;
  papel: 'Aluno' | 'Admin';
}

export interface AuthResponse {
  token: string;
  usuario: Usuario;
  expiraEm: string;
}

export interface LoginRequest {
  email: string;
  senha: string;
}

export interface RegistroRequest {
  nome: string;
  email: string;
  senha: string;
}

export interface Linha {
  id: number;
  codigo: string;
  nome: string;
  descricao?: string;
  nivelLotacaoAtual?: NivelLotacao;
}

export interface Parada {
  id: number;
  nome: string;
  latitude: number;
  longitude: number;
  distanciaMetros?: number;
}

export interface RotaParada extends Parada {
  ordem: number;
}

export interface Rota {
  linhaId: number;
  linhaCodigo: string;
  linhaNome: string;
  paradas: RotaParada[];
}

export interface NovoReporte {
  linhaId: number;
  paradaId?: number | null;
  nivelLotacao: NivelLotacao;
  latitude?: number | null;
  longitude?: number | null;
}

export interface Reporte {
  id: number;
  linhaId: number;
  linhaNome: string;
  paradaId?: number | null;
  nivelLotacao: NivelLotacao;
  criadoEm: string;
  valido: boolean;
}

export interface LinhaSuperlotacao {
  linhaNome: string;
  percentualSuperlotado: number;
}

export interface PontoCalor {
  paradaId: number;
  paradaNome: string;
  latitude: number;
  longitude: number;
  intensidade: number;
}

export interface FaixaEspera {
  faixaHoraria: string;
  minutosEsperaEstimados: number;
}

export interface DashboardKpis {
  totalReportesHoje: number;
  totalLinhasMonitoradas: number;
  linhasComMaiorSuperlotacao: LinhaSuperlotacao[];
  mapaDeCalor: PontoCalor[];
  previsaoTempoEspera: FaixaEspera[];
}

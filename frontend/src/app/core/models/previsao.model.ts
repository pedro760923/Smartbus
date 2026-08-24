import { NivelLotacao } from './linha.model';

export interface Previsao {
  linhaId: number;
  diaSemana: number;
  faixaHoraria: string;
  nivelPrevisto: NivelLotacao;
  confiabilidade: number;
  amostras: number;
}

export interface DashboardKpis {
  totalReportesHoje: number;
  totalLinhasMonitoradas: number;
  linhasComMaiorSuperlotacao: { linhaNome: string; percentualSuperlotado: number }[];
  mapaDeCalor: { paradaId: number; paradaNome: string; latitude: number; longitude: number; intensidade: number }[];
  previsaoTempoEspera: { faixaHoraria: string; minutosEsperaEstimados: number }[];
}

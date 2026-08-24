import { NivelLotacao } from './linha.model';

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

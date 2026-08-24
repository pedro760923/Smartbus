import { Parada } from './parada.model';

export interface Rota {
  linhaId: number;
  linhaCodigo: string;
  linhaNome: string;
  paradas: (Parada & { ordem: number })[];
}

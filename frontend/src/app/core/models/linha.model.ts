export interface Linha {
  id: number;
  codigo: string;
  nome: string;
  descricao?: string;
  nivelLotacaoAtual?: NivelLotacao;
}

export enum NivelLotacao {
  Vazio = 0,
  ComLugares = 1,
  Cheio = 2,
  SuperLotado = 3
}

export const NIVEL_LOTACAO_LABEL: Record<NivelLotacao, string> = {
  [NivelLotacao.Vazio]: 'Vazio',
  [NivelLotacao.ComLugares]: 'Com lugares',
  [NivelLotacao.Cheio]: 'Cheio',
  [NivelLotacao.SuperLotado]: 'Superlotado'
};

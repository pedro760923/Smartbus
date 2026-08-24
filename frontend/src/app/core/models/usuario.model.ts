export interface Usuario {
  id: number;
  nome: string;
  email: string;
  papel: 'Aluno' | 'Admin';
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

export interface AuthResponse {
  token: string;
  usuario: Usuario;
  expiraEm: string;
}

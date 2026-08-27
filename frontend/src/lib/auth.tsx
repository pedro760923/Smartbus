import { createContext, useCallback, useContext, useMemo, useState, type ReactNode } from 'react';
import { api } from './api';
import type { AuthResponse, LoginRequest, RegistroRequest, Usuario } from '../types';

const TOKEN_KEY = 'smartbus_token';
const USUARIO_KEY = 'smartbus_usuario';

interface AuthContextValue {
  usuario: Usuario | null;
  autenticado: boolean;
  isAdmin: boolean;
  login: (dados: LoginRequest) => Promise<Usuario>;
  registrar: (dados: RegistroRequest) => Promise<Usuario>;
  logout: () => void;
}

const AuthContext = createContext<AuthContextValue | null>(null);

function carregarUsuarioLocal(): Usuario | null {
  const raw = localStorage.getItem(USUARIO_KEY);
  return raw ? (JSON.parse(raw) as Usuario) : null;
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [usuario, setUsuario] = useState<Usuario | null>(carregarUsuarioLocal);

  const persistirSessao = useCallback((resp: AuthResponse) => {
    localStorage.setItem(TOKEN_KEY, resp.token);
    localStorage.setItem(USUARIO_KEY, JSON.stringify(resp.usuario));
    setUsuario(resp.usuario);
  }, []);

  const login = useCallback(
    async (dados: LoginRequest) => {
      const { data } = await api.post<AuthResponse>('/auth/login', dados);
      persistirSessao(data);
      return data.usuario;
    },
    [persistirSessao]
  );

  const registrar = useCallback(
    async (dados: RegistroRequest) => {
      const { data } = await api.post<AuthResponse>('/auth/registro', dados);
      persistirSessao(data);
      return data.usuario;
    },
    [persistirSessao]
  );

  const logout = useCallback(() => {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USUARIO_KEY);
    setUsuario(null);
  }, []);

  const value = useMemo<AuthContextValue>(
    () => ({
      usuario,
      autenticado: usuario != null,
      isAdmin: usuario?.papel === 'Admin',
      login,
      registrar,
      logout
    }),
    [usuario, login, registrar, logout]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth(): AuthContextValue {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth deve ser usado dentro de <AuthProvider>.');
  return ctx;
}

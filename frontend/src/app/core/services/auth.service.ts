import { Injectable, computed, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AuthResponse, LoginRequest, RegistroRequest, Usuario } from '../models/usuario.model';

const TOKEN_KEY = 'smartbus_token';
const USUARIO_KEY = 'smartbus_usuario';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly _usuario = signal<Usuario | null>(this.carregarUsuarioLocal());
  readonly usuario = computed(() => this._usuario());
  readonly autenticado = computed(() => !!this._usuario());
  readonly isAdmin = computed(() => this._usuario()?.papel === 'Admin');

  constructor(private http: HttpClient) {}

  login(dados: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${environment.apiUrl}/auth/login`, dados).pipe(
      tap((resp) => this.persistirSessao(resp))
    );
  }

  registrar(dados: RegistroRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${environment.apiUrl}/auth/registro`, dados).pipe(
      tap((resp) => this.persistirSessao(resp))
    );
  }

  logout(): void {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USUARIO_KEY);
    this._usuario.set(null);
  }

  obterToken(): string | null {
    return localStorage.getItem(TOKEN_KEY);
  }

  private persistirSessao(resp: AuthResponse): void {
    localStorage.setItem(TOKEN_KEY, resp.token);
    localStorage.setItem(USUARIO_KEY, JSON.stringify(resp.usuario));
    this._usuario.set(resp.usuario);
  }

  private carregarUsuarioLocal(): Usuario | null {
    const raw = localStorage.getItem(USUARIO_KEY);
    return raw ? (JSON.parse(raw) as Usuario) : null;
  }
}

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Rota } from '../models/rota.model';

@Injectable({ providedIn: 'root' })
export class RotaService {
  private readonly base = `${environment.apiUrl}/rotas`;

  constructor(private http: HttpClient) {}

  obterPorLinha(linhaId: number): Observable<Rota> {
    return this.http.get<Rota>(`${this.base}/${linhaId}`);
  }
}

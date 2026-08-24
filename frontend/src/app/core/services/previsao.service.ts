import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Previsao } from '../models/previsao.model';

@Injectable({ providedIn: 'root' })
export class PrevisaoService {
  private readonly base = `${environment.apiUrl}/previsao`;

  constructor(private http: HttpClient) {}

  obterPorLinha(linhaId: number): Observable<Previsao> {
    return this.http.get<Previsao>(`${this.base}/linha/${linhaId}`);
  }
}

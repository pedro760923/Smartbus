import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Linha } from '../models/linha.model';

@Injectable({ providedIn: 'root' })
export class LinhaService {
  private readonly base = `${environment.apiUrl}/linhas`;

  constructor(private http: HttpClient) {}

  listar(termo?: string): Observable<Linha[]> {
    const params = termo ? { termo } : undefined;
    return this.http.get<Linha[]>(this.base, { params });
  }

  obterPorId(id: number): Observable<Linha> {
    return this.http.get<Linha>(`${this.base}/${id}`);
  }
}

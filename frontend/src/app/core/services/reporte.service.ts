import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { NovoReporte, Reporte } from '../models/reporte.model';

@Injectable({ providedIn: 'root' })
export class ReporteService {
  private readonly base = `${environment.apiUrl}/reportes`;

  constructor(private http: HttpClient) {}

  enviar(reporte: NovoReporte): Observable<Reporte> {
    return this.http.post<Reporte>(this.base, reporte);
  }

  recentesPorLinha(linhaId: number): Observable<Reporte[]> {
    return this.http.get<Reporte[]>(`${this.base}/linha/${linhaId}/recentes`);
  }
}

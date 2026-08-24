import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Parada } from '../models/parada.model';

@Injectable({ providedIn: 'root' })
export class ParadaService {
  private readonly base = `${environment.apiUrl}/paradas`;

  constructor(private http: HttpClient) {}

  proximas(latitude: number, longitude: number, raioMetros = 1000): Observable<Parada[]> {
    return this.http.get<Parada[]>(`${this.base}/proximas`, {
      params: { latitude, longitude, raioMetros }
    });
  }

  listar(): Observable<Parada[]> {
    return this.http.get<Parada[]>(this.base);
  }
}

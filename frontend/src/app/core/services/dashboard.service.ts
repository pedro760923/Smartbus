import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { DashboardKpis } from '../models/previsao.model';

@Injectable({ providedIn: 'root' })
export class DashboardService {
  private readonly base = `${environment.apiUrl}/dashboard`;

  constructor(private http: HttpClient) {}

  obterKpis(): Observable<DashboardKpis> {
    return this.http.get<DashboardKpis>(`${this.base}/kpis`);
  }
}

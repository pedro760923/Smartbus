import { AfterViewInit, Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import * as L from 'leaflet';
import { HeaderComponent } from '../../shared/components/header/header.component';
import { DashboardService } from '../../core/services/dashboard.service';
import { DashboardKpis } from '../../core/models/previsao.model';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, HeaderComponent],
  template: `
    <app-header titulo="Painel administrativo"></app-header>

    <main class="dashboard" *ngIf="kpis as k">
      <section class="kpis">
        <div class="kpi">
          <span class="kpi__valor">{{ k.totalReportesHoje }}</span>
          <span class="kpi__label">Reportes hoje</span>
        </div>
        <div class="kpi">
          <span class="kpi__valor">{{ k.totalLinhasMonitoradas }}</span>
          <span class="kpi__label">Linhas monitoradas</span>
        </div>
      </section>

      <section class="ranking">
        <h3>Linhas com maior superlotação</h3>
        <ul>
          @for (item of k.linhasComMaiorSuperlotacao; track item.linhaNome) {
            <li>
              <span>{{ item.linhaNome }}</span>
              <strong>{{ item.percentualSuperlotado }}%</strong>
            </li>
          }
        </ul>
      </section>

      <section class="previsao-espera">
        <h3>Previsão de tempo de espera</h3>
        <ul>
          @for (item of k.previsaoTempoEspera; track item.faixaHoraria) {
            <li>
              <span>{{ item.faixaHoraria }}</span>
              <strong>{{ item.minutosEsperaEstimados }} min</strong>
            </li>
          }
        </ul>
      </section>

      <section class="mapa-calor">
        <h3>Mapa de calor de ocupação</h3>
        <div #mapaEl class="mapa-calor__mapa"></div>
      </section>
    </main>

    @if (!kpis) {
      <p class="carregando">Carregando indicadores...</p>
    }
  `,
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit, AfterViewInit, OnDestroy {
  @ViewChild('mapaEl') mapaEl?: ElementRef<HTMLDivElement>;

  kpis: DashboardKpis | null = null;
  private mapa?: L.Map;

  constructor(private dashboardService: DashboardService) {}

  ngOnInit(): void {
    this.dashboardService.obterKpis().subscribe((kpis) => {
      this.kpis = kpis;
      queueMicrotask(() => this.renderizarMapaDeCalor());
    });
  }

  ngAfterViewInit(): void {
    // O mapa é renderizado apenas após os KPIs chegarem (ver ngOnInit),
    // pois o *ngIf remove o elemento do DOM até os dados existirem.
  }

  private renderizarMapaDeCalor(): void {
    if (!this.mapaEl || !this.kpis || this.mapa) return;

    const pontos = this.kpis.mapaDeCalor;
    const centro = pontos[0] ?? { latitude: -23.6486, longitude: -46.5389 };

    this.mapa = L.map(this.mapaEl.nativeElement).setView([centro.latitude, centro.longitude], 14);

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      attribution: '&copy; OpenStreetMap contributors',
      maxZoom: 19
    }).addTo(this.mapa);

    pontos.forEach((ponto) => {
      L.circleMarker([ponto.latitude, ponto.longitude], {
        radius: 8 + ponto.intensidade * 12,
        color: '#c0392b',
        fillColor: '#e74c3c',
        fillOpacity: 0.5
      })
        .addTo(this.mapa!)
        .bindPopup(`${ponto.paradaNome} · intensidade ${(ponto.intensidade * 100).toFixed(0)}%`);
    });
  }

  ngOnDestroy(): void {
    this.mapa?.remove();
  }
}

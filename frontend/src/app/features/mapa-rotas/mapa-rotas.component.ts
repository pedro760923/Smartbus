import { AfterViewInit, Component, ElementRef, OnDestroy, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import * as L from 'leaflet';
import { HeaderComponent } from '../../shared/components/header/header.component';
import { BottomNavComponent } from '../../shared/components/bottom-nav/bottom-nav.component';
import { RotaService } from '../../core/services/rota.service';
import { Rota } from '../../core/models/rota.model';

@Component({
  selector: 'app-mapa-rotas',
  standalone: true,
  imports: [CommonModule, HeaderComponent, BottomNavComponent],
  template: `
    <app-header [titulo]="rota ? rota.linhaCodigo + ' · ' + rota.linhaNome : 'Mapa de rotas'"></app-header>

    <main class="mapa-rotas">
      <div #mapaEl class="mapa-rotas__mapa"></div>

      <div class="mapa-rotas__acoes">
        <button class="btn" (click)="irParaReporte()">Reportar lotação desta linha</button>
      </div>
    </main>

    <app-bottom-nav></app-bottom-nav>
  `,
  styleUrl: './mapa-rotas.component.scss'
})
export class MapaRotasComponent implements AfterViewInit, OnDestroy {
  @ViewChild('mapaEl') mapaEl!: ElementRef<HTMLDivElement>;

  rota: Rota | null = null;
  private mapa?: L.Map;
  private linhaId!: number;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private rotaService: RotaService
  ) {
    this.linhaId = Number(this.route.snapshot.paramMap.get('linhaId'));
  }

  ngAfterViewInit(): void {
    this.rotaService.obterPorLinha(this.linhaId).subscribe((rota) => {
      this.rota = rota;
      this.renderizarMapa(rota);
    });
  }

  private renderizarMapa(rota: Rota): void {
    const paradasOrdenadas = [...rota.paradas].sort((a, b) => a.ordem - b.ordem);
    const centro = paradasOrdenadas[0] ?? { latitude: -23.6486, longitude: -46.5389 };

    this.mapa = L.map(this.mapaEl.nativeElement).setView([centro.latitude, centro.longitude], 14);

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      attribution: '&copy; OpenStreetMap contributors',
      maxZoom: 19
    }).addTo(this.mapa);

    paradasOrdenadas.forEach((parada) => {
      L.marker([parada.latitude, parada.longitude])
        .addTo(this.mapa!)
        .bindPopup(parada.nome);
    });

    if (paradasOrdenadas.length > 1) {
      const linha = paradasOrdenadas.map((p) => [p.latitude, p.longitude] as [number, number]);
      L.polyline(linha, { color: '#1f6f43', weight: 4 }).addTo(this.mapa);
      this.mapa.fitBounds(L.latLngBounds(linha), { padding: [24, 24] });
    }
  }

  irParaReporte(): void {
    this.router.navigate(['/reportar'], { queryParams: { linhaId: this.linhaId } });
  }

  ngOnDestroy(): void {
    this.mapa?.remove();
  }
}

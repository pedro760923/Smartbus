import { AfterViewInit, Component, ElementRef, OnDestroy, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { forkJoin } from 'rxjs';
import * as L from 'leaflet';
import { LinhaService } from '../../core/services/linha.service';
import { RotaService } from '../../core/services/rota.service';
import { Linha, NivelLotacao } from '../../core/models/linha.model';

const CENTRO_PADRAO: [number, number] = [-23.6547, -46.5382];
const CORES_ROTA = ['#1e4d94', '#2fa360', '#f2b705', '#8e5fd0', '#e0473e'];

@Component({
  selector: 'app-linhas',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <main class="onibus-disponiveis">
      <button class="voltar" (click)="voltar()" aria-label="Voltar">‹</button>

      <div #mapaEl class="onibus-disponiveis__mapa"></div>

      <section class="painel">
        <div class="painel__cabecalho">
          <h3>Ônibus Disponíveis</h3>
          <input
            class="painel__busca"
            type="search"
            placeholder="Buscar por código ou nome"
            [(ngModel)]="termo"
            (ngModelChange)="buscar()"
          />
        </div>

        <ul class="painel__lista">
          @for (linha of linhas; track linha.id) {
            <li (click)="irParaMapa(linha)">
              <span class="badge-linha" [style.background]="corDaLinha(linha)">{{ linha.codigo }}</span>
              <span class="painel__info">
                <strong>{{ linha.nome }}</strong>
                <small>Ver rota e reportar lotação</small>
              </span>
              <span class="status" [class]="'status--' + statusLotacao(linha.nivelLotacaoAtual)">
                {{ labelLotacao(linha.nivelLotacaoAtual) }}
              </span>
            </li>
          }
          @empty {
            <li class="painel__vazio">Nenhuma linha encontrada.</li>
          }
        </ul>
      </section>
    </main>
  `,
  styleUrl: './linhas.component.scss'
})
export class LinhasComponent implements AfterViewInit, OnDestroy {
  @ViewChild('mapaEl') mapaEl!: ElementRef<HTMLDivElement>;

  linhas: Linha[] = [];
  termo = '';
  private mapa?: L.Map;
  private coresPorLinha = new Map<number, string>();

  constructor(
    private linhaService: LinhaService,
    private rotaService: RotaService,
    private router: Router
  ) {}

  ngAfterViewInit(): void {
    this.mapa = L.map(this.mapaEl.nativeElement).setView(CENTRO_PADRAO, 13);
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      attribution: '&copy; OpenStreetMap contributors',
      maxZoom: 19
    }).addTo(this.mapa);

    this.buscar();
  }

  buscar(): void {
    this.linhaService.listar(this.termo || undefined).subscribe((linhas) => {
      this.linhas = linhas;
      this.desenharRotas(linhas);
    });
  }

  private desenharRotas(linhas: Linha[]): void {
    if (!this.mapa || linhas.length === 0) return;

    forkJoin(linhas.map((linha) => this.rotaService.obterPorLinha(linha.id))).subscribe((rotas) => {
      const todosPontos: [number, number][] = [];

      rotas.forEach((rota, indice) => {
        const cor = CORES_ROTA[indice % CORES_ROTA.length];
        this.coresPorLinha.set(rota.linhaId, cor);

        const paradasOrdenadas = [...rota.paradas].sort((a, b) => a.ordem - b.ordem);
        const pontos = paradasOrdenadas.map((p) => [p.latitude, p.longitude] as [number, number]);
        pontos.forEach((ponto) => todosPontos.push(ponto));

        if (pontos.length > 1) {
          L.polyline(pontos, { color: cor, weight: 4 }).addTo(this.mapa!);
        }

        paradasOrdenadas.forEach((parada) => {
          L.circleMarker([parada.latitude, parada.longitude], {
            radius: 5,
            color: cor,
            fillColor: cor,
            fillOpacity: 0.9
          })
            .addTo(this.mapa!)
            .bindPopup(`${rota.linhaCodigo} · ${parada.nome}`);
        });
      });

      if (todosPontos.length > 1) {
        this.mapa!.fitBounds(L.latLngBounds(todosPontos), { padding: [24, 24] });
      }
    });
  }

  corDaLinha(linha: Linha): string {
    return this.coresPorLinha.get(linha.id) ?? '#1e4d94';
  }

  statusLotacao(nivel?: NivelLotacao): 'verde' | 'amarelo' | 'vermelho' {
    if (nivel === NivelLotacao.SuperLotado) return 'vermelho';
    if (nivel === NivelLotacao.Cheio) return 'amarelo';
    return 'verde';
  }

  labelLotacao(nivel?: NivelLotacao): string {
    if (nivel === NivelLotacao.SuperLotado) return 'Lotado';
    if (nivel === NivelLotacao.Cheio) return 'Médio';
    if (nivel === NivelLotacao.ComLugares) return 'Com lugares';
    return 'Vazio';
  }

  irParaMapa(linha: Linha): void {
    this.router.navigate(['/mapa-rotas', linha.id]);
  }

  voltar(): void {
    this.router.navigate(['/principal']);
  }

  ngOnDestroy(): void {
    this.mapa?.remove();
  }
}

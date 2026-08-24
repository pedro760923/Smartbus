import { AfterViewInit, Component, ElementRef, OnDestroy, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import * as L from 'leaflet';
import { LinhaService } from '../../core/services/linha.service';
import { RotaService } from '../../core/services/rota.service';
import { ReporteService } from '../../core/services/reporte.service';
import { AuthService } from '../../core/services/auth.service';
import { Linha, NivelLotacao } from '../../core/models/linha.model';

const CENTRO_PADRAO: [number, number] = [-23.6547, -46.5382];

@Component({
  selector: 'app-reportar-lotacao',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <main class="reportar">
      <button class="voltar" (click)="voltar()" aria-label="Voltar">‹</button>

      <div #mapaEl class="reportar__mapa"></div>

      <section class="painel">
        <h2>Como está sua viagem{{ auth.usuario()?.nome ? ', ' + auth.usuario()?.nome : '' }}?</h2>

        <label class="reportar__campo">
          <select [(ngModel)]="linhaSelecionadaId" name="linha" (ngModelChange)="onLinhaAlterada()">
            <option [ngValue]="null" disabled>Selecione a linha</option>
            @for (linha of linhas; track linha.id) {
              <option [ngValue]="linha.id">{{ linha.codigo }} · {{ linha.nome }}</option>
            }
          </select>
        </label>

        <div class="opcoes-lotacao">
          <button
            type="button"
            class="opcao opcao--verde"
            [class.selecionada]="nivelSelecionado === NivelLotacao.Vazio"
            (click)="nivelSelecionado = NivelLotacao.Vazio"
          >
            <span class="opcao__icone">🟢</span>
            Muitos assentos livres
          </button>
          <button
            type="button"
            class="opcao opcao--amarelo"
            [class.selecionada]="nivelSelecionado === NivelLotacao.ComLugares"
            (click)="nivelSelecionado = NivelLotacao.ComLugares"
          >
            <span class="opcao__icone">🟡</span>
            Poucos assentos livres
          </button>
          <button
            type="button"
            class="opcao opcao--vermelho"
            [class.selecionada]="nivelSelecionado === NivelLotacao.SuperLotado"
            (click)="nivelSelecionado = NivelLotacao.SuperLotado"
          >
            <span class="opcao__icone">🔴</span>
            Todos em pé / Lotado
          </button>
        </div>

        @if (erro) {
          <p class="erro">{{ erro }}</p>
        }

        <button
          class="btn btn--primario"
          [disabled]="linhaSelecionadaId == null || nivelSelecionado == null || enviando"
          (click)="enviar()"
        >
          {{ enviando ? 'Enviando...' : 'Enviar Reporte' }}
        </button>
      </section>
    </main>
  `,
  styleUrl: './reportar-lotacao.component.scss'
})
export class ReportarLotacaoComponent implements AfterViewInit, OnDestroy {
  @ViewChild('mapaEl') mapaEl!: ElementRef<HTMLDivElement>;

  linhas: Linha[] = [];
  linhaSelecionadaId: number | null = null;
  nivelSelecionado: NivelLotacao | null = null;
  NivelLotacao = NivelLotacao;
  erro = '';
  enviando = false;
  private mapa?: L.Map;

  constructor(
    private linhaService: LinhaService,
    private rotaService: RotaService,
    private reporteService: ReporteService,
    private route: ActivatedRoute,
    private router: Router,
    public auth: AuthService
  ) {}

  ngAfterViewInit(): void {
    this.mapa = L.map(this.mapaEl.nativeElement).setView(CENTRO_PADRAO, 14);
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      attribution: '&copy; OpenStreetMap contributors',
      maxZoom: 19
    }).addTo(this.mapa);

    this.linhaService.listar().subscribe((linhas) => {
      this.linhas = linhas;
      const linhaIdQuery = this.route.snapshot.queryParamMap.get('linhaId');
      if (linhaIdQuery) {
        this.linhaSelecionadaId = Number(linhaIdQuery);
        this.onLinhaAlterada();
      }
    });
  }

  onLinhaAlterada(): void {
    if (this.linhaSelecionadaId == null || !this.mapa) return;

    this.rotaService.obterPorLinha(this.linhaSelecionadaId).subscribe((rota) => {
      this.mapa!.eachLayer((camada) => {
        if (!(camada instanceof L.TileLayer)) this.mapa!.removeLayer(camada);
      });

      const paradasOrdenadas = [...rota.paradas].sort((a, b) => a.ordem - b.ordem);
      const pontos = paradasOrdenadas.map((p) => [p.latitude, p.longitude] as [number, number]);

      paradasOrdenadas.forEach((parada) => {
        L.marker([parada.latitude, parada.longitude]).addTo(this.mapa!).bindPopup(parada.nome);
      });

      if (pontos.length > 1) {
        L.polyline(pontos, { color: '#1e4d94', weight: 4 }).addTo(this.mapa!);
        this.mapa!.fitBounds(L.latLngBounds(pontos), { padding: [24, 24] });
      }
    });
  }

  enviar(): void {
    if (this.linhaSelecionadaId == null || this.nivelSelecionado == null) return;
    this.enviando = true;
    this.erro = '';

    const concluir = (latitude?: number, longitude?: number) => {
      this.reporteService
        .enviar({
          linhaId: this.linhaSelecionadaId!,
          nivelLotacao: this.nivelSelecionado!,
          latitude,
          longitude
        })
        .subscribe({
          next: () => this.router.navigate(['/confirmacao']),
          error: () => {
            this.erro = 'Não foi possível enviar seu reporte. Tente novamente.';
            this.enviando = false;
          }
        });
    };

    if ('geolocation' in navigator) {
      navigator.geolocation.getCurrentPosition(
        (posicao) => concluir(posicao.coords.latitude, posicao.coords.longitude),
        () => concluir(undefined, undefined)
      );
    } else {
      concluir(undefined, undefined);
    }
  }

  voltar(): void {
    this.router.navigate(['/linhas']);
  }

  ngOnDestroy(): void {
    this.mapa?.remove();
  }
}

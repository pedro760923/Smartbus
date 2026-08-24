import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ParadaService } from '../../core/services/parada.service';
import { Parada } from '../../core/models/parada.model';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-principal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <main class="home">
      <header class="home__topo">
        <svg width="30" height="30" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
          <rect x="3" y="4" width="18" height="12" rx="4" fill="#1e4d94"/>
          <circle cx="7.5" cy="18.5" r="1.6" fill="#1e4d94"/>
          <circle cx="16.5" cy="18.5" r="1.6" fill="#1e4d94"/>
          <path d="M6 8h12M6 11.5h8" stroke="#fff" stroke-width="1.4" stroke-linecap="round"/>
        </svg>
        <div class="home__topo-marca">
          <strong>SmartBus</strong>
          <span>Santo André</span>
        </div>
        <button class="home__sair" (click)="sair()" aria-label="Sair">Sair</button>
      </header>

      <label class="home__busca">
        <span class="home__busca-icone">🔍</span>
        <input
          type="search"
          placeholder="Para onde vamos?"
          [(ngModel)]="termoBusca"
          name="busca"
        />
      </label>

      <button class="cartao-destaque" (click)="irParaOnibusDisponiveis()">
        <span class="cartao-destaque__icone">🚌</span>
        <span class="cartao-destaque__texto">
          <strong>Ônibus disponíveis</strong>
          <small>Veja o mapa e a lotação em tempo real</small>
        </span>
        <span class="cartao-destaque__seta">›</span>
      </button>

      <section class="home__paradas">
        <div class="home__paradas-cabecalho">
          <h3>Paradas Próximas</h3>
          @if (carregando) {
            <small>localizando...</small>
          }
        </div>

        @if (erroLocalizacao) {
          <p class="aviso">
            Não foi possível acessar sua localização. Ative o GPS/permissão do navegador
            para ver as paradas mais próximas.
          </p>
        }

        <ul class="lista-paradas">
          @for (parada of paradasFiltradas(); track parada.id) {
            <li (click)="irParaOnibusDisponiveis()">
              <span class="lista-paradas__icone">🚏</span>
              <span class="lista-paradas__info">
                <strong>{{ parada.nome }}</strong>
                @if (parada.distanciaMetros != null) {
                  <small>{{ parada.distanciaMetros | number: '1.0-0' }} m de você</small>
                }
              </span>
              <span class="lista-paradas__seta">›</span>
            </li>
          }
          @empty {
            @if (!carregando && !erroLocalizacao) {
              <li class="lista-paradas__vazio">Nenhuma parada encontrada nas proximidades.</li>
            }
          }
        </ul>
      </section>

      <section class="cartao-avaliacao">
        @if (avaliacaoEnviada) {
          <p class="cartao-avaliacao__obrigado">Obrigado pela sua avaliação! ⭐</p>
        } @else {
          <p class="cartao-avaliacao__titulo">⭐ Avalie sua experiência</p>
          <div class="cartao-avaliacao__estrelas">
            @for (estrela of [1, 2, 3, 4, 5]; track estrela) {
              <span
                (click)="notaSelecionada = estrela"
                [class.ativa]="estrela <= notaSelecionada"
              >★</span>
            }
          </div>
          <button
            class="btn btn--primario"
            [disabled]="notaSelecionada === 0"
            (click)="avaliacaoEnviada = true"
          >Enviar</button>
        }
      </section>
    </main>
  `,
  styleUrl: './principal.component.scss'
})
export class PrincipalComponent implements OnInit {
  paradas: Parada[] = [];
  carregando = false;
  erroLocalizacao = false;
  termoBusca = '';
  notaSelecionada = 0;
  avaliacaoEnviada = false;

  constructor(
    private paradaService: ParadaService,
    private router: Router,
    public auth: AuthService
  ) {}

  ngOnInit(): void {
    this.buscarParadasProximas();
  }

  paradasFiltradas(): Parada[] {
    const termo = this.termoBusca.trim().toLowerCase();
    if (!termo) return this.paradas;
    return this.paradas.filter((p) => p.nome.toLowerCase().includes(termo));
  }

  irParaOnibusDisponiveis(): void {
    this.router.navigate(['/linhas']);
  }

  sair(): void {
    this.auth.logout();
    this.router.navigate(['/boas-vindas']);
  }

  private buscarParadasProximas(): void {
    if (!('geolocation' in navigator)) {
      this.erroLocalizacao = true;
      return;
    }

    this.carregando = true;
    navigator.geolocation.getCurrentPosition(
      (posicao) => {
        this.paradaService
          .proximas(posicao.coords.latitude, posicao.coords.longitude)
          .subscribe({
            next: (paradas) => {
              this.paradas = paradas;
              this.carregando = false;
            },
            error: () => {
              this.carregando = false;
            }
          });
      },
      () => {
        this.erroLocalizacao = true;
        this.carregando = false;
      }
    );
  }
}
